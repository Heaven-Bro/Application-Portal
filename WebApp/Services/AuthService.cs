using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace WebApp.Services;

public sealed class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(
        HttpClient httpClient, 
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", new 
            { 
                email, 
                password 
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                
                try
                {
                    var error = JsonSerializer.Deserialize<ApiError>(errorContent, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return LoginResult.Failure(error?.Error ?? "Invalid email or password");
                }
                catch
                {
                    return LoginResult.Failure("Invalid email or password");
                }
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            
            if (loginResponse == null)
                return LoginResult.Failure("Invalid response from server");

            await StoreAuthDataAsync(
                loginResponse.Token,
                loginResponse.User.Email,
                loginResponse.User.Role,
                loginResponse.User.Username,
                loginResponse.User.Id
            );

            await NotifyAuthenticationStateChangedAsync();

            return LoginResult.Success();
        }
        catch (HttpRequestException)
        {
            return LoginResult.Failure("Cannot connect to server. Check your internet connection.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Login failed: {ex.Message}");
            return LoginResult.Failure("An unexpected error occurred");
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await ClearAuthDataAsync();
            await NotifyAuthenticationStateChangedAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Logout failed: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            var email = await _localStorage.GetItemAsync<string>("userEmail");
            
            return !string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(email);
        }
        catch
        {
            return false;
        }
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        try
        {
            var email = await _localStorage.GetItemAsync<string>("userEmail");
            
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var role = await _localStorage.GetItemAsync<string>("userRole");
            var username = await _localStorage.GetItemAsync<string>("userName");
            var userId = await _localStorage.GetItemAsync<int>("userId");

            return new UserInfo
            {
                Email = email,
                Role = role ?? "User",
                Username = username ?? email,
                UserId = userId
            };
        }
        catch
        {
            return null;
        }
    }

    private async Task StoreAuthDataAsync(string token, string email, string role, string username, int userId)
    {
        await _localStorage.SetItemAsync("authToken", token);
        await _localStorage.SetItemAsync("userEmail", email);
        await _localStorage.SetItemAsync("userRole", role);
        await _localStorage.SetItemAsync("userName", username);
        await _localStorage.SetItemAsync("userId", userId);
    }

    private async Task ClearAuthDataAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("userEmail");
        await _localStorage.RemoveItemAsync("userRole");
        await _localStorage.RemoveItemAsync("userName");
        await _localStorage.RemoveItemAsync("userId");
    }

    private async Task NotifyAuthenticationStateChangedAsync()
    {
        if (_authStateProvider is AppAuthStateProvider provider)
        {
            await provider.NotifyAuthenticationStateChangedAsync();
        }
    }
}

// DTOs
public sealed class LoginResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    private LoginResult(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static LoginResult Success() => new(true);
    public static LoginResult Failure(string errorMessage) => new(false, errorMessage);
}

public sealed class UserInfo
{
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public int UserId { get; init; }
}

internal sealed class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
}

internal sealed class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
}

internal sealed class ApiError
{
    public string Error { get; set; } = string.Empty;
}

// Auth State Provider
public sealed class AppAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public AppAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(_anonymous);

            var email = await _localStorage.GetItemAsync<string>("userEmail");
            
            if (string.IsNullOrWhiteSpace(email))
                return new AuthenticationState(_anonymous);

            var role = await _localStorage.GetItemAsync<string>("userRole");
            var username = await _localStorage.GetItemAsync<string>("userName");

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username ?? email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task NotifyAuthenticationStateChangedAsync()
    {
        var authState = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }
}
