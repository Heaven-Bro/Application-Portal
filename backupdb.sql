-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3307
-- Generation Time: Feb 06, 2026 at 10:32 PM
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
-- Database: `backupdb`
--

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`` PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY` (IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))   BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END$$

CREATE DEFINER=`` PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY` (IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))   BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `applications`
--

CREATE TABLE `applications` (
  `Id` bigint(20) NOT NULL,
  `ServiceId` bigint(20) NOT NULL,
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
  `Description` varchar(1000) DEFAULT NULL,
  `Condition` int(11) NOT NULL DEFAULT 0,
  `IsAvailable` tinyint(1) NOT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `equipment`
--

INSERT INTO `equipment` (`Id`, `Name`, `EquipmentCode`, `Category`, `Description`, `Condition`, `IsAvailable`, `Version`, `CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`) VALUES
(3, 'Desktop Computer Lab-234', 'COMP-203-01', 'Computer', 'Intel i7, 16GB Ram, 4GB RTX graphics card. Display has some issue', 0, 1, 13, 0, '2026-02-06 12:27:03.544394', 0, '2026-02-06 16:02:31.153235'),
(5, 'Desktop Computer Lab-234', 'COMP-203-02', 'Computer', 'Core i3 10gen, 4gb ram, no dedicated graphics card\n', 2, 0, 2, 0, '2026-02-06 12:55:09.342547', 0, '2026-02-06 13:09:00.732508');

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
-- Table structure for table `notifications`
--

CREATE TABLE `notifications` (
  `Id` bigint(20) NOT NULL,
  `UserId` bigint(20) NOT NULL COMMENT 'Admin who needs to see this',
  `Type` int(11) NOT NULL COMMENT '0=EquipmentOverdue, 1=NewApplication, 2=StepSubmitted',
  `ReferenceId` bigint(20) DEFAULT NULL COMMENT 'EquipmentAssignmentId or ApplicationId',
  `Message` varchar(500) NOT NULL,
  `IsRead` tinyint(1) NOT NULL DEFAULT 0,
  `ReadAt` datetime(6) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ExpiresAt` datetime(6) DEFAULT NULL COMMENT 'Auto-dismiss after date'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `services`
--

CREATE TABLE `services` (
  `Id` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Description` varchar(1000) NOT NULL,
  `ServiceType` int(11) NOT NULL DEFAULT 0 COMMENT '0=ChairmanAppointment, 1=EquipmentLoan, 2=GeneralApplication',
  `IsActive` tinyint(1) NOT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `services`
--

INSERT INTO `services` (`Id`, `Name`, `Description`, `ServiceType`, `IsActive`, `Version`, `CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`) VALUES
(3, 'Chairman Sir\'s Appointment', 'You will get chairman sir\'s appointment only on those free slot shown on the Calendar. It is on the top of the homepage', 0, 1, 1, 0, '2026-02-04 18:38:12.852694', NULL, NULL),
(5, 'Certificate Course Application', 'Certification of Computer/Non-Departmental on basis the Application', 2, 1, 1, 2, '2026-02-04 21:40:14.455936', NULL, NULL),
(7, 'Equipment Access', 'Apply with equipment list and an application including the reference of supervisor or chairman sir.', 1, 1, 1, 2, '2026-02-06 13:40:16.734467', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `service_steps`
--

CREATE TABLE `service_steps` (
  `Id` bigint(20) NOT NULL,
  `ServiceId` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Order` int(11) NOT NULL,
  `RequiresFileUpload` tinyint(1) NOT NULL,
  `RequiresTextInput` tinyint(1) NOT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL,
  `DownloadableFormUrl` varchar(500) DEFAULT NULL,
  `Instructions` varchar(2000) DEFAULT NULL,
  `UploadConfig` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`UploadConfig`)),
  `IsOptional` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `service_steps`
--

INSERT INTO `service_steps` (`Id`, `ServiceId`, `Name`, `Order`, `RequiresFileUpload`, `RequiresTextInput`, `Version`, `CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`, `DownloadableFormUrl`, `Instructions`, `UploadConfig`, `IsOptional`) VALUES
(4, 3, 'Tell Us the reason', 1, 0, 1, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, NULL, 'This is instruction 1', NULL, 0),
(5, 3, 'Upload Application or Documents', 2, 1, 0, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, NULL, 'This is Instruction 2..', '{\"Label\":\"Upload Application or Documents\",\"Instructions\":\"Instruction for file upload\",\"AllowedTypes\":[\"pdf\",\"jpg\",\"png\"],\"MaxFiles\":5}', 0),
(7, 5, 'Download this form', 1, 0, 0, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, 'https://drive.google.com/file/d/1MA4GC3Cf2qmExt5LpuJympmL_y4qo6Hs/view', 'Fill the form and take corresponding NOC', NULL, 0),
(8, 5, 'Upload the form', 2, 1, 0, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, NULL, 'Take reference from the Chairman Sir and Upload it.', '{\"Label\":\"Upload Form\",\"Instructions\":\"Upload pdf or photo. Avoid larger files\",\"AllowedTypes\":[\"pdf\",\"jpg\",\"png\"],\"MaxFiles\":3}', 0),
(9, 5, 'Payment', 3, 1, 0, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, NULL, 'Pay 200 Taka on the account 0991783862673763', '{\"Label\":\"Upload Receipt\",\"Instructions\":\"Upload bank receipts. Use clear image so that we can verify.\",\"AllowedTypes\":[\"pdf\",\"png\",\"jpg\"],\"MaxFiles\":2}', 0),
(11, 7, 'Upload Application and List', 1, 1, 0, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, 'google.com', 'An application (inc. chairman sir or supervisor signature) with the equipment list you want to access', '{\"Label\":\"Upload Documents\",\"Instructions\":\"Upload Application with lists. Multiple files can be uploaded though avoid larger files\",\"AllowedTypes\":[\"pdf\",\"png\",\"jpg\"],\"MaxFiles\":3}', 0);

-- --------------------------------------------------------

--
-- Table structure for table `step_submissions`
--

CREATE TABLE `step_submissions` (
  `Id` bigint(20) NOT NULL,
  `ApplicationId` bigint(20) NOT NULL,
  `StepId` bigint(20) NOT NULL,
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
-- Table structure for table `step_submission_documents`
--

CREATE TABLE `step_submission_documents` (
  `Id` bigint(20) NOT NULL,
  `StepSubmissionId` bigint(20) NOT NULL,
  `UserDocumentId` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL
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
-- Table structure for table `user_documents`
--

CREATE TABLE `user_documents` (
  `Id` bigint(20) NOT NULL,
  `UserId` bigint(20) NOT NULL,
  `ApplicationId` bigint(20) DEFAULT NULL COMMENT 'Optional: links to specific application',
  `FileName` varchar(255) NOT NULL,
  `OriginalFileName` varchar(255) NOT NULL COMMENT 'User-friendly display name',
  `FilePath` varchar(1000) NOT NULL,
  `FileType` varchar(50) NOT NULL COMMENT 'application/pdf, image/jpeg, etc',
  `FileSize` bigint(20) NOT NULL COMMENT 'In bytes',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `Version` int(11) NOT NULL DEFAULT 0,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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
('20251215153938_AddNameToUser', '9.0.0'),
('20260204154326_AddStepInstructionsAndUpload', '9.0.0'),
('20260204175744_AddIsOptionalToServiceStep', '9.0.0'),
('20260204181347_RemoveDescriptionFromServiceStep', '9.0.0'),
('20260204194504_AddCreatedByToService', '9.0.0');

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
  ADD KEY `IX_equipment_IsAvailable` (`IsAvailable`),
  ADD KEY `IX_equipment_Condition` (`Condition`);

--
-- Indexes for table `notifications`
--
ALTER TABLE `notifications`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `idx_notifications_userid_isread` (`UserId`,`IsRead`),
  ADD KEY `idx_notifications_createdat` (`CreatedAt`);

--
-- Indexes for table `services`
--
ALTER TABLE `services`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_services_IsActive_ServiceVersion` (`IsActive`);

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
-- Indexes for table `step_submission_documents`
--
ALTER TABLE `step_submission_documents`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `unique_submission_document` (`StepSubmissionId`,`UserDocumentId`),
  ADD KEY `idx_step_submission_documents_submissionid` (`StepSubmissionId`),
  ADD KEY `idx_step_submission_documents_documentid` (`UserDocumentId`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `IX_users_Email` (`Email`),
  ADD UNIQUE KEY `IX_users_Username` (`Username`);

--
-- Indexes for table `user_documents`
--
ALTER TABLE `user_documents`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `idx_user_documents_userid` (`UserId`),
  ADD KEY `idx_user_documents_applicationid` (`ApplicationId`);

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
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `notifications`
--
ALTER TABLE `notifications`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `services`
--
ALTER TABLE `services`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `service_steps`
--
ALTER TABLE `service_steps`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `step_submissions`
--
ALTER TABLE `step_submissions`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `step_submission_documents`
--
ALTER TABLE `step_submission_documents`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `user_documents`
--
ALTER TABLE `user_documents`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `notifications`
--
ALTER TABLE `notifications`
  ADD CONSTRAINT `FK_notifications_users` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE;

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

--
-- Constraints for table `step_submission_documents`
--
ALTER TABLE `step_submission_documents`
  ADD CONSTRAINT `FK_step_submission_documents_documents` FOREIGN KEY (`UserDocumentId`) REFERENCES `user_documents` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_step_submission_documents_submissions` FOREIGN KEY (`StepSubmissionId`) REFERENCES `step_submissions` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `user_documents`
--
ALTER TABLE `user_documents`
  ADD CONSTRAINT `FK_user_documents_applications` FOREIGN KEY (`ApplicationId`) REFERENCES `applications` (`Id`) ON DELETE SET NULL,
  ADD CONSTRAINT `FK_user_documents_users` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
