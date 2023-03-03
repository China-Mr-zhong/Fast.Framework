/*
 Navicat Premium Data Transfer

 Source Server         : 本地Sqlite
 Source Server Type    : SQLite
 Source Server Version : 3035005
 Source Schema         : main

 Target Server Type    : SQLite
 Target Server Version : 3035005
 File Encoding         : 65001

 Date: 01/02/2023 15:35:02
*/

PRAGMA foreign_keys = false;

-- ----------------------------
-- Table structure for Category
-- ----------------------------
DROP TABLE IF EXISTS "Category";
CREATE TABLE "Category" (
  "CategoryId" INTeger PRIMARY KEY AUTOINCREMENT,
  "CategoryName" VARCHAR(50)
);

PRAGMA foreign_keys = true;

/*
 Navicat Premium Data Transfer

 Source Server         : 本地Sqlite
 Source Server Type    : SQLite
 Source Server Version : 3035005
 Source Schema         : main

 Target Server Type    : SQLite
 Target Server Version : 3035005
 File Encoding         : 65001

 Date: 01/02/2023 15:35:10
*/

PRAGMA foreign_keys = false;

-- ----------------------------
-- Table structure for Product
-- ----------------------------
DROP TABLE IF EXISTS "Product";
CREATE TABLE "Product" (
  "ProductId" INTeger PRIMARY KEY AUTOINCREMENT,
  "CategoryId" INTeger,
  "ProductCode" VARCHAR(50),
  "ProductName" VARCHAR(100),
  "CreateTime" DATETIME,
  "ModifyTime" DATETIME,
  "DeleteMark" INTeger,
  "Custom1" VARCHAR(50),
  "Custom2" VARCHAR(50),
  "Custom3" VARCHAR(50),
  "Custom4" VARCHAR(50),
  "Custom5" VARCHAR(50),
  "Custom6" VARCHAR(50),
  "Custom7" VARCHAR(50),
  "Custom8" VARCHAR(50),
  "Custom9" VARCHAR(50),
  "Custom10" VARCHAR(50),
  "Custom11" VARCHAR(50),
  "Custom12" VARCHAR(50)
);

PRAGMA foreign_keys = true;
