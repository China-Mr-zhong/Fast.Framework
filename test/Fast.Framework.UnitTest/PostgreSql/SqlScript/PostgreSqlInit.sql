/*
 Navicat Premium Data Transfer

 Source Server         : 本地PostgreSql
 Source Server Type    : PostgreSQL
 Source Server Version : 140005
 Source Host           : localhost:5432
 Source Catalog        : test
 Source Schema         : public

 Target Server Type    : PostgreSQL
 Target Server Version : 140005
 File Encoding         : 65001

 Date: 01/02/2023 13:31:44
*/


-- ----------------------------
-- Table structure for Category
-- ----------------------------
DROP TABLE IF EXISTS "public"."Category";
CREATE TABLE "public"."Category" (
  "CategoryId" int4 NOT NULL GENERATED ALWAYS AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "CategoryName" varchar(50) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Primary Key structure for table Category
-- ----------------------------
ALTER TABLE "public"."Category" ADD CONSTRAINT "Category_pkey" PRIMARY KEY ("CategoryId");

/*
 Navicat Premium Data Transfer

 Source Server         : 本地PostgreSql
 Source Server Type    : PostgreSQL
 Source Server Version : 140005
 Source Host           : localhost:5432
 Source Catalog        : test
 Source Schema         : public

 Target Server Type    : PostgreSQL
 Target Server Version : 140005
 File Encoding         : 65001

 Date: 01/02/2023 13:31:51
*/


-- ----------------------------
-- Table structure for Product
-- ----------------------------
DROP TABLE IF EXISTS "public"."Product";
CREATE TABLE "public"."Product" (
  "ProductId" int4 NOT NULL GENERATED ALWAYS AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "CategoryId" int4,
  "ProductCode" varchar(50) COLLATE "pg_catalog"."default",
  "ProductName" varchar(100) COLLATE "pg_catalog"."default",
  "CreateTime" timestamp(6),
  "ModifyTime" timestamp(6),
  "DeleteMark" bit,
  "Custom1" varchar(50) COLLATE "pg_catalog"."default",
  "Custom2" varchar(50) COLLATE "pg_catalog"."default",
  "Custom3" varchar(50) COLLATE "pg_catalog"."default",
  "Custom4" varchar(50) COLLATE "pg_catalog"."default",
  "Custom5" varchar(50) COLLATE "pg_catalog"."default",
  "Custom6" varchar(50) COLLATE "pg_catalog"."default",
  "Custom7" varchar(50) COLLATE "pg_catalog"."default",
  "Custom8" varchar(50) COLLATE "pg_catalog"."default",
  "Custom9" varchar(50) COLLATE "pg_catalog"."default",
  "Custom10" varchar(50) COLLATE "pg_catalog"."default",
  "Custom11" varchar(50) COLLATE "pg_catalog"."default",
  "Custom12" varchar(50) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Primary Key structure for table Product
-- ----------------------------
ALTER TABLE "public"."Product" ADD CONSTRAINT "Product_pkey" PRIMARY KEY ("ProductId");
