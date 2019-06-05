namespace OxfordStreet_online_app.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_product_name : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Name");
        }
    }
}
