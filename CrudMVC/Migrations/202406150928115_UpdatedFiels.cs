namespace CrudMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedFiels : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserProfiles", "Email", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserProfiles", "Email", c => c.String());
        }
    }
}
