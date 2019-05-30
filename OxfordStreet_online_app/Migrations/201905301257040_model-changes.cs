namespace OxfordStreet_online_app.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modelchanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "Role", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "Role", c => c.String());
        }
    }
}
