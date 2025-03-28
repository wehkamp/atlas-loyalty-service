# Migrations 

## Folder structure
To remove clutter, the migration files are stored in folders per year

## Versioning and conventions
Version number format: `yyyyMMddHHmm`

When creating a new migration, also include the version number and a short description of what the migration is doing, for example:

```
202301011500_AddColumnAToTableB.cs
```

The contents of the file would look something like this:

```csharp
[Migration(202301011500)]
public class AddColumnAToTableB : UpMigration
{
    public override void Up()
    {
        Alter.Table("B").AddColumn("A").AsString(32);
    }
}
```


