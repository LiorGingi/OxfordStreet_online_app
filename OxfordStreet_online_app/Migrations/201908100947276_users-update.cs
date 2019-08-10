namespace OxfordStreet_online_app.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usersupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Address", c => c.String());
            AddColumn("dbo.Users", "PhoneNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "PhoneNumber");
            DropColumn("dbo.Users", "Address");
        }
    }
}
