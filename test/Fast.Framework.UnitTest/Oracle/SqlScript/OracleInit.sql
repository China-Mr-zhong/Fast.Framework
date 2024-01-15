DROP sequence mySeq;

create sequence mySeq
START WITH 1
INCREMENT BY 1
NOMAXVALUE;

-- ----------------------------
-- Table structure for Category
-- ----------------------------
DROP TABLE "SYSTEM"."Category";
CREATE TABLE "SYSTEM"."Category" (
  "CategoryId" NUMBER VISIBLE NOT NULL,
  "CategoryName" VARCHAR2(50 BYTE) VISIBLE
)
LOGGING
NOCOMPRESS
PCTFREE 10
INITRANS 1
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  FREELISTS 1
  FREELIST GROUPS 1
  BUFFER_POOL DEFAULT
)
PARALLEL 1
NOCACHE
DISABLE ROW MOVEMENT
;

-- ----------------------------
-- Primary Key structure for table Category
-- ----------------------------
ALTER TABLE "SYSTEM"."Category" ADD CONSTRAINT "SYS_C008113" PRIMARY KEY ("CategoryId");


-- ----------------------------
-- Table structure for Product
-- ----------------------------
DROP TABLE "SYSTEM"."Product";
CREATE TABLE "SYSTEM"."Product" (
  "ProductId" NUMBER(10,0) VISIBLE NOT NULL,
  "CategoryId" NUMBER(10,0) VISIBLE,
  "ProductCode" VARCHAR2(50 BYTE) VISIBLE,
  "ProductName" VARCHAR2(100 BYTE) VISIBLE,
  "CreateTime" TIMESTAMP(6) VISIBLE,
  "ModifyTime" TIMESTAMP(6) VISIBLE,
  "DeleteMark" NUMBER(1,0) VISIBLE,
  "Custom1" VARCHAR2(50 BYTE) VISIBLE,
  "Custom2" VARCHAR2(50 BYTE) VISIBLE,
  "Custom3" VARCHAR2(50 BYTE) VISIBLE,
  "Custom4" VARCHAR2(50 BYTE) VISIBLE,
  "Custom5" VARCHAR2(50 BYTE) VISIBLE,
  "Custom6" VARCHAR2(50 BYTE) VISIBLE,
  "Custom7" VARCHAR2(50 BYTE) VISIBLE,
  "Custom8" VARCHAR2(50 BYTE) VISIBLE,
  "Custom9" VARCHAR2(50 BYTE) VISIBLE,
  "Custom10" VARCHAR2(50 BYTE) VISIBLE,
  "Custom11" VARCHAR2(50 BYTE) VISIBLE,
  "Custom12" VARCHAR2(50 BYTE) VISIBLE
)
LOGGING
NOCOMPRESS
PCTFREE 10
INITRANS 1
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  FREELISTS 1
  FREELIST GROUPS 1
  BUFFER_POOL DEFAULT
)
PARALLEL 1
NOCACHE
DISABLE ROW MOVEMENT
;

-- ----------------------------
-- Primary Key structure for table Product
-- ----------------------------
ALTER TABLE "SYSTEM"."Product" ADD CONSTRAINT "SYS_C008116" PRIMARY KEY ("ProductId");


DROP trigger trg_Category_insert;

create trigger trg_Category_insert before insert on "Category" for each row
begin
select mySeq.nextval into :new."CategoryId" from dual;
end;

DROP trigger trg_Product_insert;

create trigger trg_Product_insert before insert on "Product" for each row
begin
select mySeq.nextval into :new."ProductId" from dual;
end;