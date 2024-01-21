using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family.Budget.Infrastructure.Migrations
{
    public partial class DatabaseInitialValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    INSERT INTO budget.""Configuration""
    (""Id"", ""Name"", ""Value"", ""Description"", ""StartDate"", ""FinalDate"", ""CreatedAt"", ""CreatedBy"")
    VALUES
    ('41f5f916-596e-4381-98f1-62f8a77aa1a2', 'subcategories_default', 'c24acdb1-2897-48cf-bb9e-c5531493817b,5e211458-23b9-4039-9dc2-276831b4898e,355d81b0-e61c-4364-aa02-f1f45083c14c,2a6083f0-46e0-4020-8c11-0574fb233b04,099f8f85-830c-4711-bedf-3f8bcb6e608f,1370e6ba-3454-45cd-a8ef-00a514062281,d8938ce6-0906-4b44-9576-8cbdec4dae18,c5dbe44f-a726-4433-8177-c1e09298d73f,ecf967cb-0d8a-4401-8c33-667c2926d8ff,183fff27-adbc-4ca2-9769-03c2d62bf5e2,cb064858-5fdf-417f-9c0e-926f0cddfa90,5a800f23-1de0-41d3-b7fe-f198d7101f5d,934831fc-3a1a-4e76-b83f-3eab2a8442b4,bb9db1f0-765b-4c8b-a582-ffc18eecadb1,a5f40ccb-5d9b-45c3-bd0d-5bcfefe21fde,343c37c6-ffab-4676-9fc9-baa680821907,3a9220f1-b88f-45b0-9f7a-125024734156,b18efdd9-0471-498c-97a9-117cc7d6f5b1,07d2886b-755e-41f0-bba5-666b758c0a4b,3c1da1cc-f42a-4f47-af46-2d047936c303,e324efc4-137b-4de7-b413-193c980a3e84,8b953444-b56b-4f7a-a18d-b6172c166c56,cf7689ab-41cd-492f-9a01-fecaa82f4cb2,5142df27-edc6-4fe9-8530-a8ef08f13f6a,ce01c625-c112-43c3-a990-3c157367dae8,2b11bc58-655a-4be9-9fd8-225ed2b13bbf,2a4d6017-1a3b-4d33-9bbd-076b49183260,966f5091-2b89-45a3-b2d8-0f2fe99512e8,ceeb5d7a-17fc-4d5a-9dd2-037381802506,969902e0-3052-4d40-abe1-beaf6cc9d5ee,8ba87af7-ea9b-4033-b160-ee215e710f8d,32b4643d-163c-41d4-9bc4-a8b1e2c0eaa1,7c49a988-d266-4955-be5e-bb58c5d05318,32783b35-b2ba-4f4a-ad74-5082292bdd56,d00e7053-4383-4d85-9dce-797d4baa34fb,8aeb1ed6-d042-49aa-8311-022f09f43e08,d6cfe9d5-a18d-4256-9840-91ae498b27b6,507dc46d-1180-4b91-b49e-3c5c9b771468,55cbe641-e644-4b91-8fc1-0c5e4ec07c0a,f44d9413-5245-4d53-b725-fbba130654b2,8f84ab7a-e2ee-4233-95a3-2554e9aa7a7d,e68506a2-f500-48f8-a915-f48026d574f4,f65f4f8f-2558-4bff-a06e-e382937f93f6,b46e4020-f460-4070-9e92-e8c9f9774c87,1b56ca8b-ba30-476f-919d-ec7ba970cc71,7f7007ca-2739-440b-89c1-1a8a65a8e95f,b331e327-2950-4271-aeb3-fadad062858f,603399c7-0960-4063-b595-b8c5289730ef,735e7e23-3cd4-4c06-aee2-2ad7ae5bcef4,b66aad18-5f6d-467f-a43b-4383b2fe3015,36266214-f43d-4470-839d-ecd58380d0ef,210212d5-4229-4a29-9501-401a089edb34,687f3df5-baad-45db-8ba0-a958fbce1913', 'subcategories_default', NOW(), null, NOW(), 'System'),
    ('8b01ade8-0467-4352-af66-4af750030e00', 'defaultLocation:language', 'en', 'defaultLocation:language', NOW(), null, NOW(), 'System'),
    ('e5352c57-9e44-4a88-afca-c54c3de49bad', 'defaultLocation:currency', '6959d265-7efa-4d1a-a07a-2f8bc51fa0b8', 'defaultLocation:currency', NOW(), null, NOW(), 'System'),
    ('d07ed0f3-4c6f-4d7f-a4fd-979aafe79217', 'register_email', '3fae1937-2742-42fe-8370-3d974bccab73', 'register_email', NOW(), null, NOW(), 'System'),
    ('e4860f82-ca17-4b00-b282-4f802cc84e26', 'wellcome_email', '3fae1937-2742-42fe-8370-3d974bccab73', 'wellcome_email', NOW(), null, NOW(), 'System'),
    ('03fd2958-60cf-4fe6-848d-1ebcc0bfc281', 'paymentmethods_default', 'b05246de-8fc0-481a-80be-9197f6db3fb2,71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6,45e87b93-f001-44b4-a7a6-a099618a3042,ffad05f8-5748-498c-8002-1d1624107fc7,5d44c865-5e56-4f12-926c-6c97a44cc2a9,b72a24e3-7c5f-4790-9a3b-959ddf3c8315,93cef651-35c5-4486-81e4-a61962695b81,d463630d-308b-4b88-a764-cb8592ac93d1,a2bf0c1f-941d-41e0-9dc1-578e7c232e2f,0d49c9c6-ee52-4670-8654-bf0d819e0b80,4e8263ee-0769-486f-ac80-a1804e28f729,3a32cefe-aeb7-465a-a63f-883346df8639', 'wellcome_email', NOW(), null, NOW(), 'System');

    INSERT INTO budget.""Currency""
    (""Id"", ""Name"", ""Iso"", ""Symbol"")
    VALUES('6959d265-7efa-4d1a-a07a-2f8bc51fa0b8', 'Dolar', 'DLR', '$');

    INSERT INTO budget.""Category""
    (""Id"", ""Name"", ""Description"", ""StartDate"", ""DeactivationDate"", ""Type"")
    VALUES 
    ('cc1b00df-79fc-4058-915e-81c9eb2ba76e','Feeding','feeding',null,null,2),
    ('271dd7e2-80c9-4d66-94f5-75e74dc90bc8','Habitation','habitation',null,null,2),
    ('97487a15-cc62-4452-8743-0492c8bcf70e','Education','',null,null,2),
    ('b276a770-4a78-4a2d-aa5f-02ea0f6b9e63','Communication','',null,null,2),
    ('4550f2fc-c1aa-4caf-9e93-934e783885ac','Health','',null,null,2),
    ('f615eb79-4510-49a1-b695-4602e9010519','Transport','',null,null,2),
    ('2a266c4f-ad87-4f77-825b-1945b503a393','Personal','',null,null,2),
    ('e863073c-6d4e-47c0-9c3e-bb1e7accd7f0','Leisure','',null,null,2),
    ('c7588d95-44a9-4199-95e5-775f5ff2cec2','Financial services','',null,null,2);

    INSERT INTO budget.""SubCategory""
    (""Id"", ""Name"", ""Description"", ""StartDate"", ""DeactivationDate"", ""CategoryId"")
    VALUES 
    ('c24acdb1-2897-48cf-bb9e-c5531493817b','Supermarket','',null,null,'cc1b00df-79fc-4058-915e-81c9eb2ba76e'),
    ('5e211458-23b9-4039-9dc2-276831b4898e','Fair / Sacolão','',null,null,'cc1b00df-79fc-4058-915e-81c9eb2ba76e'),
    ('355d81b0-e61c-4364-aa02-f1f45083c14c','Bakery','',null,null,'cc1b00df-79fc-4058-915e-81c9eb2ba76e'),
    ('2a6083f0-46e0-4020-8c11-0574fb233b04','Food - Other','',null,null,'cc1b00df-79fc-4058-915e-81c9eb2ba76e'),
    ('099f8f85-830c-4711-bedf-3f8bcb6e608f','Rent','',null,null,'271dd7e2-80c9-4d66-94f5-75e74dc90bc8'),
    ('1370e6ba-3454-45cd-a8ef-00a514062281','Gas','',null,null,'271dd7e2-80c9-4d66-94f5-75e74dc90bc8'),
    ('d8938ce6-0906-4b44-9576-8cbdec4dae18','Water','',null,null,'271dd7e2-80c9-4d66-94f5-75e74dc90bc8'),
    ('c5dbe44f-a726-4433-8177-c1e09298d73f','Laundry','',null,null,'271dd7e2-80c9-4d66-94f5-75e74dc90bc8'),
    ('ecf967cb-0d8a-4401-8c33-667c2926d8ff','Electricity','',null,null,'271dd7e2-80c9-4d66-94f5-75e74dc90bc8'),
    ('183fff27-adbc-4ca2-9769-03c2d62bf5e2','Others','',null,null,'271dd7e2-80c9-4d66-94f5-75e74dc90bc8'),
    ('cb064858-5fdf-417f-9c0e-926f0cddfa90','IPTU','',null,null,'271dd7e2-80c9-4d66-94f5-75e74dc90bc8'),
    ('5a800f23-1de0-41d3-b7fe-f198d7101f5d','House makeover','',null,null,'271dd7e2-80c9-4d66-94f5-75e74dc90bc8'),
    ('934831fc-3a1a-4e76-b83f-3eab2a8442b4','Monthly payment','',null,null,'97487a15-cc62-4452-8743-0492c8bcf70e'),
    ('bb9db1f0-765b-4c8b-a582-ffc18eecadb1','School supplies','',null,null,'97487a15-cc62-4452-8743-0492c8bcf70e'),
    ('a5f40ccb-5d9b-45c3-bd0d-5bcfefe21fde','Other courses','',null,null,'97487a15-cc62-4452-8743-0492c8bcf70e'),
    ('343c37c6-ffab-4676-9fc9-baa680821907','School bus','',null,null,'97487a15-cc62-4452-8743-0492c8bcf70e'),
    ('3a9220f1-b88f-45b0-9f7a-125024734156','Cell phone','',null,null,'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63'),
    ('b18efdd9-0471-498c-97a9-117cc7d6f5b1','Netflix','',null,null,'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63'),
    ('07d2886b-755e-41f0-bba5-666b758c0a4b','Internet','',null,null,'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63'),
    ('3c1da1cc-f42a-4f47-af46-2d047936c303','Cable TV','',null,null,'b276a770-4a78-4a2d-aa5f-02ea0f6b9e63'),
    ('e324efc4-137b-4de7-b413-193c980a3e84','Academy','',null,null,'4550f2fc-c1aa-4caf-9e93-934e783885ac'),
    ('8b953444-b56b-4f7a-a18d-b6172c166c56','Medicines','',null,null,'4550f2fc-c1aa-4caf-9e93-934e783885ac'),
    ('cf7689ab-41cd-492f-9a01-fecaa82f4cb2','Health insurance','',null,null,'4550f2fc-c1aa-4caf-9e93-934e783885ac'),
    ('5142df27-edc6-4fe9-8530-a8ef08f13f6a','Perfumes','',null,null,'4550f2fc-c1aa-4caf-9e93-934e783885ac'),
    ('ce01c625-c112-43c3-a990-3c157367dae8','Bath','',null,null,'4550f2fc-c1aa-4caf-9e93-934e783885ac'),
    ('2b11bc58-655a-4be9-9fd8-225ed2b13bbf','Others','',null,null,'f615eb79-4510-49a1-b695-4602e9010519'),
    ('2a4d6017-1a3b-4d33-9bbd-076b49183260','Taxi','',null,null,'f615eb79-4510-49a1-b695-4602e9010519'),
    ('966f5091-2b89-45a3-b2d8-0f2fe99512e8','Fuel','',null,null,'f615eb79-4510-49a1-b695-4602e9010519'),
    ('ceeb5d7a-17fc-4d5a-9dd2-037381802506','Parking','',null,null,'f615eb79-4510-49a1-b695-4602e9010519'),
    ('969902e0-3052-4d40-abe1-beaf6cc9d5ee','Safe','',null,null,'f615eb79-4510-49a1-b695-4602e9010519'),
    ('8ba87af7-ea9b-4033-b160-ee215e710f8d','Maintenance','',null,null,'f615eb79-4510-49a1-b695-4602e9010519'),
    ('32b4643d-163c-41d4-9bc4-a8b1e2c0eaa1','Subway','',null,null,'f615eb79-4510-49a1-b695-4602e9010519'),
    ('7c49a988-d266-4955-be5e-bb58c5d05318','Toll','',null,null,'f615eb79-4510-49a1-b695-4602e9010519'),
    ('32783b35-b2ba-4f4a-ad74-5082292bdd56','IUC / Licensing','',null,null,'f615eb79-4510-49a1-b695-4602e9010519'),
    ('d00e7053-4383-4d85-9dce-797d4baa34fb','Clothing and footwear','',null,null,'2a266c4f-ad87-4f77-825b-1945b503a393'),
    ('8aeb1ed6-d042-49aa-8311-022f09f43e08','Hairdresser / Manicure','',null,null,'2a266c4f-ad87-4f77-825b-1945b503a393'),
    ('d6cfe9d5-a18d-4256-9840-91ae498b27b6','Manicure','',null,null,'2a266c4f-ad87-4f77-825b-1945b503a393'),
    ('507dc46d-1180-4b91-b49e-3c5c9b771468','Gifts','',null,null,'2a266c4f-ad87-4f77-825b-1945b503a393'),
    ('55cbe641-e644-4b91-8fc1-0c5e4ec07c0a','Others','',null,null,'2a266c4f-ad87-4f77-825b-1945b503a393'),
    ('f44d9413-5245-4d53-b725-fbba130654b2','Cinema / Theater','',null,null,'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0'),
    ('8f84ab7a-e2ee-4233-95a3-2554e9aa7a7d','Books / Magazines / CDs','',null,null,'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0'),
    ('e68506a2-f500-48f8-a915-f48026d574f4','Transportation costs','',null,null,'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0'),
    ('f65f4f8f-2558-4bff-a06e-e382937f93f6','Trips','',null,null,'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0'),
    ('b46e4020-f460-4070-9e92-e8c9f9774c87','Restaurants and Bars','',null,null,'e863073c-6d4e-47c0-9c3e-bb1e7accd7f0'),
    ('1b56ca8b-ba30-476f-919d-ec7ba970cc71','Loans','',null,null,'c7588d95-44a9-4199-95e5-775f5ff2cec2'),
    ('7f7007ca-2739-440b-89c1-1a8a65a8e95f','Insurance','',null,null,'c7588d95-44a9-4199-95e5-775f5ff2cec2'),
    ('b331e327-2950-4271-aeb3-fadad062858f','private pension','',null,null,'c7588d95-44a9-4199-95e5-775f5ff2cec2'),
    ('603399c7-0960-4063-b595-b8c5289730ef','Interest Overdraft Check','',null,null,'c7588d95-44a9-4199-95e5-775f5ff2cec2'),
    ('735e7e23-3cd4-4c06-aee2-2ad7ae5bcef4','Bank fees','',null,null,'c7588d95-44a9-4199-95e5-775f5ff2cec2'),
    ('b66aad18-5f6d-467f-a43b-4383b2fe3015','Social Security','',null,null,'c7588d95-44a9-4199-95e5-775f5ff2cec2'),
    ('36266214-f43d-4470-839d-ecd58380d0ef','Credit card bill','',null,null,'c7588d95-44a9-4199-95e5-775f5ff2cec2'),
    ('210212d5-4229-4a29-9501-401a089edb34','IRPF','',null,null,'c7588d95-44a9-4199-95e5-775f5ff2cec2'),
    ('687f3df5-baad-45db-8ba0-a958fbce1913','Investments','',null,null,'c7588d95-44a9-4199-95e5-775f5ff2cec2');

    INSERT INTO budget.""PaymentMethod""
    (""Id"", ""Name"", ""Description"", ""StartDate"", ""DeactivationDate"", ""Type"")
    VALUES
    ('b05246de-8fc0-481a-80be-9197f6db3fb2','(a) Cash','',null,null,1),
    ('71ebb6ae-42f2-4c90-9b4c-c8f8841d40a6','(b) Debit Card','',null,null,1),
    ('45e87b93-f001-44b4-a7a6-a099618a3042','(c) Transfer','',null,null,1),
    ('ffad05f8-5748-498c-8002-1d1624107fc7','(d) Automatic Debit','',null,null,1),
    ('5d44c865-5e56-4f12-926c-6c97a44cc2a9','(e) Withheld at Source','',null,null,1),
    ('b72a24e3-7c5f-4790-9a3b-959ddf3c8315','(f) Credit Card Bill this month','',null,null,1),
    ('93cef651-35c5-4486-81e4-a61962695b81','(g) Credit Card next month','',null,null,1),
    ('d463630d-308b-4b88-a764-cb8592ac93d1','Salary / Advance','',null,null,2),
    ('a2bf0c1f-941d-41e0-9dc1-578e7c232e2f','Vacation','',null,null,2),
    ('0d49c9c6-ee52-4670-8654-bf0d819e0b80','13th salary','',null,null,2),
    ('4e8263ee-0769-486f-ac80-a1804e28f729','food allowance','',null,null,2),
    ('3a32cefe-aeb7-465a-a63f-883346df8639','Extra income (rent, other)','',null,null,2);

    INSERT INTO budget.""Category""
    (""Id"", ""Name"", ""Description"", ""StartDate"", ""DeactivationDate"", ""Type"")
    VALUES 
    ('6bb017a1-7e65-4d38-a7bb-347e25c656f3','Financial Income','Financial Income',null,null,1);

    INSERT INTO budget.""SubCategory""
    (""Id"", ""Name"", ""Description"", ""StartDate"", ""DeactivationDate"", ""CategoryId"")
    VALUES 
    ('172fcd0d-4f01-4650-8e17-407aca64709d','Salary / Advance','Salary / Advance',null,null,'6bb017a1-7e65-4d38-a7bb-347e25c656f3'),
    ('b09fc2ec-380f-4cea-bf1b-fca58ef7b5cc','Vacation','',null,null,'6bb017a1-7e65-4d38-a7bb-347e25c656f3'),
    ('0a06fd0c-3a7d-4bbd-a553-8d6a5ba1347c','13th salary','',null,null,'6bb017a1-7e65-4d38-a7bb-347e25c656f3'),
    ('9ca9083f-ed80-40ec-bc33-596a35674455','Food Allowance','',null,null,'6bb017a1-7e65-4d38-a7bb-347e25c656f3'),
    ('967374c1-9c77-4d22-9b0a-aeea12d4984d','Extra income (rent, other)','',null,null,'6bb017a1-7e65-4d38-a7bb-347e25c656f3');

    update budget.""Configuration"" set ""Value"" = concat(""Value"",',172fcd0d-4f01-4650-8e17-407aca64709d,b09fc2ec-380f-4cea-bf1b-fca58ef7b5cc,0a06fd0c-3a7d-4bbd-a553-8d6a5ba1347c,9ca9083f-ed80-40ec-bc33-596a35674455,967374c1-9c77-4d22-9b0a-aeea12d4984d')
    where ""Id"" = '41f5f916-596e-4381-98f1-62f8a77aa1a2';");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
