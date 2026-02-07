-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3307
-- Generation Time: Feb 07, 2026 at 08:58 AM
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
  `AdminNotes` varchar(2000) DEFAULT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `applications`
--

INSERT INTO `applications` (`Id`, `ServiceId`, `ApplicantId`, `Status`, `CurrentStep`, `ScheduledDateTime`, `CompletedAt`, `AdminNotes`, `Version`, `CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`) VALUES
(6, 3, 1, 4, 2, NULL, NULL, 'documents not matched', 21, 1, '2026-02-06 19:03:04.421267', 2, '2026-02-06 23:03:16.417208'),
(7, 5, 1, 3, 3, '2026-02-20 17:00:00.000000', NULL, 'come to our office', 8, 1, '2026-02-06 21:20:43.764323', 2, '2026-02-06 23:05:31.555907'),
(8, 7, 1, 3, 4, '2026-02-07 08:15:00.000000', NULL, 'come to the office', 11, 1, '2026-02-06 22:04:20.335482', 2, '2026-02-07 00:10:18.170917'),
(9, 7, 3, 3, 2, '2026-02-07 09:00:00.000000', NULL, 'come to the pffice to get it', 5, 3, '2026-02-07 01:33:17.831541', 2, '2026-02-07 01:35:26.655121'),
(16, 3, 3, 3, 2, '2026-02-14 09:12:00.000000', NULL, 'come to office', 4, 3, '2026-02-07 02:10:18.754221', 2, '2026-02-07 02:11:32.305807'),
(17, 5, 3, 4, 3, NULL, NULL, 'not matched', 7, 3, '2026-02-07 02:25:30.146174', 2, '2026-02-07 02:26:48.468814');

-- --------------------------------------------------------

--
-- Table structure for table `chairman_availability`
--

CREATE TABLE `chairman_availability` (
  `Id` bigint(20) NOT NULL,
  `StartTime` datetime(6) NOT NULL,
  `EndTime` datetime(6) NOT NULL,
  `Version` int(11) NOT NULL,
  `CreatedBy` bigint(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `chairman_availability`
--

INSERT INTO `chairman_availability` (`Id`, `StartTime`, `EndTime`, `Version`, `CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`) VALUES
(1, '2026-02-02 15:00:00.000000', '2026-02-02 17:00:00.000000', 1, 2, '2026-02-07 01:17:42.733312', NULL, NULL),
(2, '2026-02-03 15:00:00.000000', '2026-02-03 17:00:00.000000', 1, 2, '2026-02-07 01:17:42.733327', NULL, NULL),
(3, '2026-02-07 09:00:00.000000', '2026-02-07 17:00:00.000000', 1, 2, '2026-02-07 01:17:42.733327', NULL, NULL),
(4, '2026-02-08 09:00:00.000000', '2026-02-08 12:00:00.000000', 1, 2, '2026-02-07 01:17:42.733327', NULL, NULL);

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
(5, 'Desktop Computer Lab-234', 'COMP-203-02', 'Computer', 'Core i3 10gen, 4gb ram, no dedicated graphics card\n', 1, 0, 6, 0, '2026-02-06 12:55:09.342547', 2, '2026-02-07 01:35:02.841640');

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

--
-- Dumping data for table `equipmentassignments`
--

INSERT INTO `equipmentassignments` (`Id`, `ApplicationId`, `EquipmentId`, `AssignedAt`, `ExpectedReturnDate`, `ReturnRequestedAt`, `ReturnVerifiedAt`, `ReturnVerifiedBy`, `Status`, `AdminNotes`, `ApplicantResponse`, `DamageAcknowledgedAt`, `Version`, `CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`) VALUES
(0, 8, 5, '2026-02-07 00:09:22.458826', '2026-02-07 00:00:00.000000', '2026-02-07 00:28:53.245327', '2026-02-07 00:40:30.777969', 2, 4, NULL, NULL, NULL, 4, 2, '2026-02-07 00:09:22.458940', 2, '2026-02-07 00:40:30.778119'),
(0, 9, 5, '2026-02-07 01:35:02.841502', '2026-02-07 00:00:00.000000', NULL, NULL, NULL, 0, NULL, NULL, NULL, 1, 2, '2026-02-07 01:35:02.841563', NULL, NULL);

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
  `ExpiresAt` datetime(6) DEFAULT NULL COMMENT 'Auto-dismiss after date',
  `Version` int(11) NOT NULL DEFAULT 1,
  `CreatedBy` bigint(20) NOT NULL DEFAULT 0,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `ModifiedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `notifications`
--

INSERT INTO `notifications` (`Id`, `UserId`, `Type`, `ReferenceId`, `Message`, `IsRead`, `ReadAt`, `CreatedAt`, `ExpiresAt`, `Version`, `CreatedBy`, `ModifiedBy`, `ModifiedAt`) VALUES
(1, 2, 1, 16, 'New application submitted by Rafid Ahmed for Chairman Sir\'s Appointment.', 1, '2026-02-07 02:11:05.346485', '2026-02-07 02:10:18.947903', NULL, 2, 3, 2, '2026-02-07 02:11:05.346528'),
(2, 2, 2, 16, 'Rafid Ahmed submitted step \'Tell Us the reason\' for Chairman Sir\'s Appointment.', 1, '2026-02-07 02:12:05.886910', '2026-02-07 02:10:35.365785', NULL, 2, 3, 2, '2026-02-07 02:12:05.886912'),
(3, 3, 10, 16, 'Your application has been approved. Appointment scheduled for Feb 14, 2026 at 09:12 AM.', 1, '2026-02-07 02:11:55.298426', '2026-02-07 02:11:32.313217', NULL, 2, 2, 3, '2026-02-07 02:11:55.298428'),
(4, 2, 1, 17, 'New application submitted by Rafid Ahmed for Certificate Course Application.', 1, '2026-02-07 02:33:42.731931', '2026-02-07 02:25:30.268996', NULL, 2, 3, 2, '2026-02-07 02:33:42.731933'),
(5, 2, 2, 17, 'Rafid Ahmed submitted step \'Download this form\' for Certificate Course Application.', 1, '2026-02-07 02:33:40.574627', '2026-02-07 02:25:36.617516', NULL, 2, 3, 2, '2026-02-07 02:33:40.574763'),
(6, 2, 2, 17, 'Rafid Ahmed submitted step \'Upload the form\' for Certificate Course Application.', 1, '2026-02-07 02:33:41.610431', '2026-02-07 02:25:49.344139', NULL, 2, 3, 2, '2026-02-07 02:33:41.610432'),
(7, 2, 2, 17, 'Rafid Ahmed submitted step \'Payment\' for Certificate Course Application.', 1, '2026-02-07 02:26:28.383298', '2026-02-07 02:25:58.388469', NULL, 2, 3, 2, '2026-02-07 02:26:28.383335'),
(8, 3, 4, 17, 'Your application was rejected. Reason: not matched', 1, '2026-02-07 02:27:06.357223', '2026-02-07 02:26:48.500143', NULL, 2, 2, 3, '2026-02-07 02:27:06.357224');

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
(7, 'Equipment Access', 'Apply with equipment list and an application including the reference of supervisor or chairman sir.', 1, 1, 1, 2, '2026-02-06 13:40:16.734467', NULL, NULL),
(8, 'Appeared Certificate', 'Submit an application included the NOC', 2, 1, 1, 2, '2026-02-07 02:48:41.755458', NULL, NULL);

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
  `IsOptional` tinyint(1) NOT NULL DEFAULT 0,
  `RequiresApproval` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `service_steps`
--

INSERT INTO `service_steps` (`Id`, `ServiceId`, `Name`, `Order`, `RequiresFileUpload`, `RequiresTextInput`, `Version`, `CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`, `DownloadableFormUrl`, `Instructions`, `UploadConfig`, `IsOptional`, `RequiresApproval`) VALUES
(4, 3, 'Tell Us the reason', 1, 0, 1, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, NULL, 'This is instruction 1', NULL, 0, 0),
(5, 3, 'Upload Application or Documents', 2, 1, 0, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, NULL, 'This is Instruction 2..', '{\"Label\":\"Upload Application or Documents\",\"Instructions\":\"Instruction for file upload\",\"AllowedTypes\":[\"pdf\",\"jpg\",\"png\"],\"MaxFiles\":5}', 1, 1),
(7, 5, 'Download this form', 1, 0, 0, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, 'https://drive.google.com/file/d/1MA4GC3Cf2qmExt5LpuJympmL_y4qo6Hs/view', 'Fill the form and take corresponding NOC', NULL, 0, 0),
(8, 5, 'Upload the form', 2, 2, 0, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, NULL, 'Take reference from the Chairman Sir and Upload it.', '{\"Label\":\"Upload Form\",\"Instructions\":\"Upload pdf or photo. Avoid larger files\",\"AllowedTypes\":[\"pdf\",\"jpg\",\"png\"],\"MaxFiles\":2}', 0, 0),
(9, 5, 'Payment', 3, 3, 0, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, NULL, 'Pay 200 Taka on the account 0991783862673763', '{\"Label\":\"Upload Receipt\",\"Instructions\":\"Upload bank receipts. Use clear image so that we can verify.\",\"AllowedTypes\":[\"pdf\",\"png\",\"jpg\"],\"MaxFiles\":2}', 0, 1),
(11, 7, 'Upload Application and List', 1, 1, 0, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, 'google.com', 'An application (inc. chairman sir or supervisor signature) with the equipment list you want to access', '{\"Label\":\"Upload Documents\",\"Instructions\":\"Upload Application with lists. Multiple files can be uploaded though avoid larger files\",\"AllowedTypes\":[\"pdf\",\"png\",\"jpg\"],\"MaxFiles\":3}', 0, 1),
(12, 8, 'Upload Your Application', 1, 1, 0, 1, 0, '0001-01-01 00:00:00.000000', NULL, NULL, NULL, 'Avoid larger files. Just pdf or images.', '{\"Label\":\"Upload Documents\",\"Instructions\":\"Pdf, jpg,png max 2mb\",\"AllowedTypes\":[\"pdf\",\"jpg\",\"png\"],\"MaxFiles\":2}', 0, 1);

-- --------------------------------------------------------

--
-- Table structure for table `step_submissions`
--

CREATE TABLE `step_submissions` (
  `Id` bigint(20) NOT NULL,
  `ApplicationId` bigint(20) NOT NULL,
  `StepId` bigint(20) NOT NULL,
  `FormData` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL,
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

--
-- Dumping data for table `step_submissions`
--

INSERT INTO `step_submissions` (`Id`, `ApplicationId`, `StepId`, `FormData`, `FilePaths`, `Status`, `ReviewedBy`, `ReviewedAt`, `RejectionReason`, `IsLatest`, `Version`, `CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`) VALUES
(10, 6, 4, 'To discuss about the festival', NULL, 1, 1, '2026-02-06 20:01:32.178829', NULL, 1, 2, 1, '2026-02-06 20:01:32.178578', 1, '2026-02-06 20:01:32.178854'),
(21, 7, 7, NULL, NULL, 1, 1, '2026-02-06 21:20:58.508943', NULL, 1, 2, 1, '2026-02-06 21:20:58.508583', 1, '2026-02-06 21:20:58.508980'),
(22, 7, 8, NULL, NULL, 0, NULL, NULL, NULL, 0, 1, 1, '2026-02-06 21:22:17.073545', NULL, NULL),
(23, 6, 5, NULL, NULL, 0, NULL, NULL, NULL, 0, 1, 1, '2026-02-06 21:24:26.777724', NULL, NULL),
(24, 6, 5, NULL, NULL, 0, NULL, NULL, NULL, 0, 1, 1, '2026-02-06 21:24:43.651420', NULL, NULL),
(25, 6, 5, NULL, NULL, 0, NULL, NULL, NULL, 0, 1, 1, '2026-02-06 21:47:22.792832', NULL, NULL),
(26, 7, 8, NULL, NULL, 1, 1, '2026-02-06 22:01:36.979368', NULL, 1, 2, 1, '2026-02-06 22:01:36.979130', 1, '2026-02-06 22:01:36.979384'),
(27, 7, 9, NULL, NULL, 1, 1, '2026-02-06 22:02:12.195218', NULL, 1, 2, 1, '2026-02-06 22:02:12.195216', 1, '2026-02-06 22:02:12.195218'),
(28, 6, 5, NULL, NULL, 2, 2, '2026-02-06 23:03:16.416904', 'documents not matched', 0, 2, 1, '2026-02-06 22:02:52.660793', 2, '2026-02-06 23:03:16.417070'),
(29, 8, 11, NULL, NULL, 1, 2, '2026-02-07 00:09:55.136280', NULL, 1, 4, 1, '2026-02-06 22:05:10.958566', 2, '2026-02-07 00:09:55.136304'),
(30, 9, 11, NULL, NULL, 1, 2, '2026-02-07 01:34:26.445227', NULL, 1, 2, 3, '2026-02-07 01:33:40.306305', 2, '2026-02-07 01:34:26.445275'),
(31, 16, 4, 'to submit my project', NULL, 1, 3, '2026-02-07 02:10:35.310716', NULL, 1, 2, 3, '2026-02-07 02:10:35.310455', 3, '2026-02-07 02:10:35.310731'),
(32, 17, 7, NULL, NULL, 1, 3, '2026-02-07 02:25:36.582069', NULL, 1, 2, 3, '2026-02-07 02:25:36.581699', 3, '2026-02-07 02:25:36.582095'),
(33, 17, 8, NULL, NULL, 1, 3, '2026-02-07 02:25:49.323980', NULL, 1, 2, 3, '2026-02-07 02:25:49.323978', 3, '2026-02-07 02:25:49.323980'),
(34, 17, 9, NULL, NULL, 2, 2, '2026-02-07 02:26:48.468787', 'not matched', 0, 2, 3, '2026-02-07 02:25:58.368755', 2, '2026-02-07 02:26:48.468788');

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

--
-- Dumping data for table `step_submission_documents`
--

INSERT INTO `step_submission_documents` (`Id`, `StepSubmissionId`, `UserDocumentId`, `CreatedAt`) VALUES
(7, 22, 7, '2026-02-06 21:22:17.073544'),
(8, 23, 8, '2026-02-06 21:24:26.777723'),
(9, 24, 9, '2026-02-06 21:24:43.651419'),
(10, 26, 10, '2026-02-06 22:01:36.979114'),
(11, 26, 11, '2026-02-06 22:01:36.979130'),
(12, 27, 12, '2026-02-06 22:02:12.195215'),
(13, 28, 13, '2026-02-06 22:02:52.660792'),
(14, 28, 14, '2026-02-06 22:02:52.660793'),
(15, 29, 15, '2026-02-06 22:05:10.958565'),
(16, 29, 16, '2026-02-06 22:05:10.958565'),
(17, 30, 17, '2026-02-07 01:33:40.306288'),
(18, 33, 18, '2026-02-07 02:25:49.323959'),
(19, 34, 19, '2026-02-07 02:25:58.368738');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `Id` bigint(20) NOT NULL,
  `Username` varchar(100) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL DEFAULT 1,
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
(1, 'md.mamun-or-rashid-200101', '200101.cse@student.just.edu.bd', 1, 'NuhhZTw9Jx0q4b6hd2xnMg==.LbUUzLRNFCe3idVbY0bvNSU+RIkLhLr6uH99g8sOnFk=', 0, 0, NULL, '01707695177', 'Pabna', 'Ambotola', '3', '2020-21', '2', 'FET', 'CSE', NULL, NULL, 2, 0, '2025-12-15 15:43:02.171770', 1, '2026-02-07 01:19:12.539339', 'Md. Mamun-Or-Rashid'),
(2, 'admin', 'admin@sys.com', 1, 'gGmEnRzzRAQ6vx6GJXUMPw==.udHx1pHE+YIv6x8mmcuaOcfEqj5rwwK9u/hcgqX1cio=', 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, '2025-12-15 19:47:06.247869', NULL, NULL, 'System Admin'),
(3, 'rafidahmed-200102', '200102.cse@student.just.edu.bd', 1, '4wzdW0vnbQN+9G6thNYplQ==.wV+3nxCOwquViYFAY0r6OnAG+ohomfnfOGu8K1Yhcvs=', 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '996aa6ef82224179ac868ea07f0ab3af', '2026-02-08 01:31:55.170405', 1, 0, '2026-02-07 01:31:55.170276', NULL, NULL, 'Rafid Ahmed');

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

--
-- Dumping data for table `user_documents`
--

INSERT INTO `user_documents` (`Id`, `UserId`, `ApplicationId`, `FileName`, `OriginalFileName`, `FilePath`, `FileType`, `FileSize`, `IsDeleted`, `Version`, `CreatedBy`, `CreatedAt`, `ModifiedBy`, `ModifiedAt`) VALUES
(7, 1, NULL, '20260206212217_177f33682f75460cbdd1e803bd69b612.jpg', 'fc3d7ed8-3436-47b7-b23a-9350d24df782.jpg', '/uploads/1/20260206212217_177f33682f75460cbdd1e803bd69b612.jpg', 'image/jpeg', 198326, 0, 1, 1, '2026-02-06 21:22:17.020121', NULL, NULL),
(8, 1, NULL, '20260206212426_6d1ebee8454440ff8b3e534a8635d5ee.jpg', '577026934_4355566464721785_6131426063597416404_n.jpg', '/uploads/1/20260206212426_6d1ebee8454440ff8b3e534a8635d5ee.jpg', 'image/jpeg', 79655, 0, 1, 1, '2026-02-06 21:24:26.737746', NULL, NULL),
(9, 1, NULL, '20260206212443_81bacc4ec2b4412aa795a6e9c9434b31.png', 'Mango-1.png', '/uploads/1/20260206212443_81bacc4ec2b4412aa795a6e9c9434b31.png', 'image/png', 434920, 0, 1, 1, '2026-02-06 21:24:43.614850', NULL, NULL),
(10, 1, NULL, '20260206220136_aef64853436148a493e1ae5282bbfc48.pdf', 'HSC_cert.pdf', '/uploads/1/20260206220136_aef64853436148a493e1ae5282bbfc48.pdf', 'application/pdf', 6667070, 0, 1, 1, '2026-02-06 22:01:36.701765', NULL, NULL),
(11, 1, NULL, '20260206220136_da8e128bf123418ebf7138370ae43752.jpg', '623116923_885965217645441_3731689999653442864_n.jpg', '/uploads/1/20260206220136_da8e128bf123418ebf7138370ae43752.jpg', 'image/jpeg', 280080, 0, 1, 1, '2026-02-06 22:01:36.878630', NULL, NULL),
(12, 1, NULL, '20260206220212_3083b3dbf29f40e58727658ed8273802.jpg', '503645935_3971402486521635_3996060630159579182_n.jpg', '/uploads/1/20260206220212_3083b3dbf29f40e58727658ed8273802.jpg', 'image/jpeg', 425483, 0, 1, 1, '2026-02-06 22:02:12.136579', NULL, NULL),
(13, 1, NULL, '20260206220252_4a719ffbf0074c9590d6b866deb5ab92.png', 'Mango-1.png', '/uploads/1/20260206220252_4a719ffbf0074c9590d6b866deb5ab92.png', 'image/png', 434920, 0, 1, 1, '2026-02-06 22:02:52.589897', NULL, NULL),
(14, 1, NULL, '20260206220252_647672ee78d6459d97cfa48280489b00.jpeg', 'imgP1.jpeg', '/uploads/1/20260206220252_647672ee78d6459d97cfa48280489b00.jpeg', 'image/jpeg', 46470, 0, 1, 1, '2026-02-06 22:02:52.609302', NULL, NULL),
(15, 1, NULL, '20260206220510_1529ceea2a7b425f9e77954398e50477.png', 'he_treated_me_then_gave_locked_acc_screenshot_to_mock_me.png', '/uploads/1/20260206220510_1529ceea2a7b425f9e77954398e50477.png', 'image/png', 488518, 0, 1, 1, '2026-02-06 22:05:10.902026', NULL, NULL),
(16, 1, NULL, '20260206220510_2b04ce0db899442e89863ee9ff2f2cdc.jpg', 'chater_payement_to_me.jpg', '/uploads/1/20260206220510_2b04ce0db899442e89863ee9ff2f2cdc.jpg', 'image/jpeg', 44694, 0, 1, 1, '2026-02-06 22:05:10.925587', NULL, NULL),
(17, 3, NULL, '20260207013340_64d361b751de40f8a1bd6075de9826d0.pdf', '200101_3_2_registration.pdf', '/uploads/3/20260207013340_64d361b751de40f8a1bd6075de9826d0.pdf', 'application/pdf', 83020, 0, 1, 3, '2026-02-07 01:33:40.188421', NULL, NULL),
(18, 3, NULL, '20260207022549_c01e1bb8e98848cda6a707a1b9a76701.jpg', 'clubFee200.jpg', '/uploads/3/20260207022549_c01e1bb8e98848cda6a707a1b9a76701.jpg', 'image/jpeg', 195828, 0, 1, 3, '2026-02-07 02:25:49.267482', NULL, NULL),
(19, 3, NULL, '20260207022558_f41e756fb4774f0fb92116c3c1b25b80.jpg', 'cseFund300.jpg', '/uploads/3/20260207022558_f41e756fb4774f0fb92116c3c1b25b80.jpg', 'image/jpeg', 200694, 0, 1, 3, '2026-02-07 02:25:58.206137', NULL, NULL);

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
-- Indexes for table `chairman_availability`
--
ALTER TABLE `chairman_availability`
  ADD PRIMARY KEY (`Id`);

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
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT for table `chairman_availability`
--
ALTER TABLE `chairman_availability`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `equipment`
--
ALTER TABLE `equipment`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `notifications`
--
ALTER TABLE `notifications`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `services`
--
ALTER TABLE `services`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `service_steps`
--
ALTER TABLE `service_steps`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `step_submissions`
--
ALTER TABLE `step_submissions`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=35;

--
-- AUTO_INCREMENT for table `step_submission_documents`
--
ALTER TABLE `step_submission_documents`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `user_documents`
--
ALTER TABLE `user_documents`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

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
