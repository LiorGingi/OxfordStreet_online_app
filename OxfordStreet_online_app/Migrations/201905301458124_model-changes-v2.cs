namespace OxfordStreet_online_app.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modelchangesv2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ImageUrl", c => c.String(nullable: false));
            AlterColumn("dbo.Orders", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "Status", c => c.String(nullable: false));
            DropColumn("dbo.Products", "ImageUrl");
        }
    }
}
