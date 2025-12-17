-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3307
-- Generation Time: Dec 17, 2025 at 09:10 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `university_service`
--

-- --------------------------------------------------------

--
-- Table structure for table `applications`
--

CREATE TABLE `applications` (
  `Id` bigint(20) NOT NULL,
  `ServiceId` bigint(20) NOT NULL,
  `ServiceVersion` int(11) NOT NULL,
  `ApplicantId` bigint(20) NOT NULL,
  `Status` int(11) NOT NULL,
  `CurrentStep` int(11) NOT NULL,
  `ScheduledDateTime` datetime(6) DEFAULT NULL,
  `CompletedAt` datetime(6) DEFAULT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `equipment`
--

CREATE TABLE `equipment` (
  `Id` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `EquipmentCode` varchar(100) NOT NULL,
  `Category` varchar(100) NOT NULL,
  `IsAvailable` tinyint(1) NOT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `equipmentassignments`
--

CREATE TABLE `equipmentassignments` (
  `Id` bigint(20) NOT NULL,
  `ApplicationId` bigint(20) NOT NULL,
  `EquipmentId` bigint(20) NOT NULL,
  `AssignedAt` datetime(6) NOT NULL,
  `ExpectedReturnDate` datetime(6) DEFAULT NULL,
  `ReturnRequestedAt` datetime(6) DEFAULT NULL,
  `ReturnVerifiedAt` datetime(6) DEFAULT NULL,
  `ReturnVerifiedBy` bigint(20) DEFAULT NULL,
  `Status` int(11) NOT NULL,
  `AdminNotes` longtext DEFAULT NULL,
  `ApplicantResponse` longtext DEFAULT NULL,
  `DamageAcknowledgedAt` datetime(6) DEFAULT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `services`
--

CREATE TABLE `services` (
  `Id` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Description` varchar(1000) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `ServiceVersion` int(11) NOT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `service_steps`
--

CREATE TABLE `service_steps` (
  `Id` bigint(20) NOT NULL,
  `ServiceId` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Description` varchar(1000) NOT NULL,
  `Order` int(11) NOT NULL,
  `RequiresFileUpload` tinyint(1) NOT NULL,
  `RequiresTextInput` tinyint(1) NOT NULL,
  `IsEquipmentAssignment` tinyint(1) NOT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `step_submissions`
--

CREATE TABLE `step_submissions` (
  `Id` bigint(20) NOT NULL,
  `ApplicationId` bigint(20) NOT NULL,
  `StepId` bigint(20) NOT NULL,
  `SubmissionVersion` int(11) NOT NULL,
  `FormData` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`FormData`)),
  `FilePaths` varchar(2000) DEFAULT NULL,
  `Status` int(11) NOT NULL,
  `ReviewedBy` bigint(20) DEFAULT NULL,
  `ReviewedAt` datetime(6) DEFAULT NULL,
  `RejectionReason` varchar(1000) DEFAULT NULL,
  `IsLatest` tinyint(1) NOT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `Id` bigint(20) NOT NULL,
  `Username` varchar(100) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `Role` int(11) NOT NULL,
  `Status` int(11) NOT NULL,
  `PhotoPath` varchar(500) DEFAULT NULL,
  `Phone` varchar(20) DEFAULT NULL,
  `PermanentAddress` varchar(500) DEFAULT NULL,
  `CurrentAddress` varchar(500) DEFAULT NULL,
  `Year` varchar(50) DEFAULT NULL,
  `Session` varchar(50) DEFAULT NULL,
  `Semester` varchar(50) DEFAULT NULL,
  `Faculty` varchar(100) DEFAULT NULL,
  `Department` varchar(100) DEFAULT NULL,
  `EmailVerificationToken` varchar(100) DEFAULT NULL,
  `EmailVerificationTokenExpiry` datetime(6) DEFAULT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL,
  `Name` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`Id`, `Username`, `Email`, `EmailConfirmed`, `PasswordHash`, `Role`, `Status`, `PhotoPath`, `Phone`, `PermanentAddress`, `CurrentAddress`, `Year`, `Session`, `Semester`, `Faculty`, `Department`, `EmailVerificationToken`, `EmailVerificationTokenExpiry`, `Version`, `CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`, `Name`) VALUES
(1, 'md.mamun-or-rashid-200101', '200101.cse@student.just.edu.bd', 1, 'NuhhZTw9Jx0q4b6hd2xnMg==.LbUUzLRNFCe3idVbY0bvNSU+RIkLhLr6uH99g8sOnFk=', 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, '2025-12-15 15:43:02.171770', NULL, NULL, 'Md. Mamun-Or-Rashid'),
(2, 'admin', 'admin@sys.com', 1, 'gGmEnRzzRAQ6vx6GJXUMPw==.udHx1pHE+YIv6x8mmcuaOcfEqj5rwwK9u/hcgqX1cio=', 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, '2025-12-15 19:47:06.247869', NULL, NULL, 'System Admin');

-- --------------------------------------------------------

--
-- Table structure for table `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20251215131746_InitialCreate', '9.0.0'),
('20251215153938_AddNameToUser', '9.0.0');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `applications`
--
ALTER TABLE `applications`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_applications_ApplicantId_Status` (`ApplicantId`,`Status`),
  ADD KEY `IX_applications_CreatedAt` (`CreatedAt`);

--
-- Indexes for table `equipment`
--
ALTER TABLE `equipment`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `IX_equipment_EquipmentCode` (`EquipmentCode`),
  ADD KEY `IX_equipment_IsAvailable` (`IsAvailable`);

--
-- Indexes for table `equipmentassignments`
--
ALTER TABLE `equipmentassignments`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `services`
--
ALTER TABLE `services`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_services_IsActive_ServiceVersion` (`IsActive`,`ServiceVersion`);

--
-- Indexes for table `service_steps`
--
ALTER TABLE `service_steps`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_service_steps_ServiceId_Order` (`ServiceId`,`Order`);

--
-- Indexes for table `step_submissions`
--
ALTER TABLE `step_submissions`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_step_submissions_ApplicationId_StepId_IsLatest` (`ApplicationId`,`StepId`,`IsLatest`),
  ADD KEY `IX_step_submissions_Status` (`Status`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `IX_users_Email` (`Email`),
  ADD UNIQUE KEY `IX_users_Username` (`Username`);

--
-- Indexes for table `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `applications`
--
ALTER TABLE `applications`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `equipment`
--
ALTER TABLE `equipment`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `equipmentassignments`
--
ALTER TABLE `equipmentassignments`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `services`
--
ALTER TABLE `services`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `service_steps`
--
ALTER TABLE `service_steps`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `step_submissions`
--
ALTER TABLE `step_submissions`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `service_steps`
--
ALTER TABLE `service_steps`
  ADD CONSTRAINT `FK_service_steps_services_ServiceId` FOREIGN KEY (`ServiceId`) REFERENCES `services` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `step_submissions`
--
ALTER TABLE `step_submissions`
  ADD CONSTRAINT `FK_step_submissions_applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `applications` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
