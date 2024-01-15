/*
 Navicat Premium Data Transfer

 Source Server         : 本地Mysql
 Source Server Type    : MySQL
 Source Server Version : 80028
 Source Host           : localhost:3306
 Source Schema         : test

 Target Server Type    : MySQL
 Target Server Version : 80028
 File Encoding         : 65001

 Date: 04/09/2022 14:12:09
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;
-- ----------------------------
-- Table structure for category
-- ----------------------------
DROP TABLE IF EXISTS `Category`;
CREATE TABLE `category`  (
  `CategoryId` int NOT NULL AUTO_INCREMENT,
  `CategoryName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`CategoryId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
-- ----------------------------
-- Table structure for Product
-- ----------------------------
DROP TABLE IF EXISTS `Product`;
CREATE TABLE `Product` (
  `ProductId` int NOT NULL AUTO_INCREMENT,
   `CategoryId` int,
  `ProductCode` varchar(50) DEFAULT NULL,
  `ProductName` varchar(100) DEFAULT NULL,
  `CreateTime` datetime DEFAULT CURRENT_TIMESTAMP,
  `ModifyTime` datetime DEFAULT NULL,
	`DeleteMark` int DEFAULT '0',
	`Custom1` varchar(50),
	`Custom2` varchar(50),
	`Custom3` varchar(50),
	`Custom4` varchar(50),
	`Custom5` varchar(50),
	`Custom6` varchar(50),
	`Custom7` varchar(50),
	`Custom8` varchar(50),
	`Custom9` varchar(50),
	`Custom10` varchar(50),
	`Custom11` varchar(50),
	`Custom12` varchar(50),
  PRIMARY KEY (`ProductId`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

SET FOREIGN_KEY_CHECKS = 1;
