namespace OxfordStreet_online_app.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oap2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartItems",
                c => new
                    {
                        ItemId = c.Int(nullable: false, identity: true),
                        CartId = c.String(),
                        Quantity = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ItemId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartItems", "ProductId", "dbo.Products");
            DropIndex("dbo.CartItems", new[] { "ProductId" });
            DropTable("dbo.CartItems");
        }
    }
}
