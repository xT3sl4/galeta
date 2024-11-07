namespace ConsoleApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Autors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Imie = c.String(),
                        Nazwisko = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ksiazkas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tytul = c.String(),
                        WydawnictwoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Wydawnictwoes", t => t.WydawnictwoId, cascadeDelete: true)
                .Index(t => t.WydawnictwoId);
            
            CreateTable(
                "dbo.Wydawnictwoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nazwa = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.KsiazkaAutors",
                c => new
                    {
                        Ksiazka_Id = c.Int(nullable: false),
                        Autor_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Ksiazka_Id, t.Autor_Id })
                .ForeignKey("dbo.Ksiazkas", t => t.Ksiazka_Id, cascadeDelete: true)
                .ForeignKey("dbo.Autors", t => t.Autor_Id, cascadeDelete: true)
                .Index(t => t.Ksiazka_Id)
                .Index(t => t.Autor_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ksiazkas", "WydawnictwoId", "dbo.Wydawnictwoes");
            DropForeignKey("dbo.KsiazkaAutors", "Autor_Id", "dbo.Autors");
            DropForeignKey("dbo.KsiazkaAutors", "Ksiazka_Id", "dbo.Ksiazkas");
            DropIndex("dbo.KsiazkaAutors", new[] { "Autor_Id" });
            DropIndex("dbo.KsiazkaAutors", new[] { "Ksiazka_Id" });
            DropIndex("dbo.Ksiazkas", new[] { "WydawnictwoId" });
            DropTable("dbo.KsiazkaAutors");
            DropTable("dbo.Wydawnictwoes");
            DropTable("dbo.Ksiazkas");
            DropTable("dbo.Autors");
        }
    }
}
