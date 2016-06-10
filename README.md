# Dematt.Airy.Nhibernate.NodaTime #

[![Build status](https://ci.appveyor.com/api/projects/status/x9ed1fnac4pllcd7/branch/master?svg=true&passingText=master%20-%20passing&failingText=master%20-%20failing&pendingText=master%20-%20pending)](https://ci.appveyor.com/project/MatthewRudolph/airy-nhibernate-extensions/branch/master)
[![Build status](https://ci.appveyor.com/api/projects/status/x9ed1fnac4pllcd7/branch/dev?svg=true&passingText=dev%20-%20passing&failingText=dev%20-%20failing&pendingText=dev%20-%20pending)](https://ci.appveyor.com/project/MatthewRudolph/airy-nhibernate-extensions/branch/dev)

## About ##
Provides Noda Time support for NHibernate.  
NHibenrate Custom UserType and Custom CompositeUserType implementations of the NodaTime classes and structures.  
Allows the use of the Noda Time classes and structures in Domain or POCO objects when using NHibernate for data access.

## Features ##
### Supported Databases ###
  * Currently we only have support for Microsoft SQL Server.
  * Tested with Microsoft SQL Server 2012 but should work for:
    * Microsoft SQL Server 2008
    * Microsoft SQL Server 2008 R2
    * Microsoft SQL Server 2012
    * Microsoft SQL Server 2014
  * Database types used that are specific to Microsoft SQL Server are, datetimeoffset, datetime2 and time.
  * Other database servers may work or partially work but have not been tested.
  * For example the Instant type uses a bigint (Int64) database type which should work with all database providers.

### Supported NodaTime Types ###
  + **DateTimeZone**  
    + Implemented by DateTimeZoneTzdbType stored as a nvarchar(35)
  + **Instant**  
    + Implemented by InstantType stored as a bigint
  + **LocalDate**  
    + Implemented by LocalDateType stored as a date
  + **LocalDateTime**  
    + Implemented by LocalDateTimeType stored as a datetime2
  + **LocalTime**  
    + Implemented by LocalTimeType stored as a time
  + **Offset**
    + Implemented by OffsetType stored as a int
  + **OffsetDateTime**  
    + Implemented by OffsetDateTimeType stored as a datetimeoffset(7)
  + **ZonedDateTime**  
    + Implemented by ZonedDateTimeBclType stored as  
      + Column1: datetimeoffset(7) (Stores the DateTime and Offset)  
      + Column2: nvarchar(35) (Stores the Bcl DateTimeZone Id)  
    + Implemented by ZonedDateTimeTzdbType stored as  
      + Column1: datetimeoffset(7) (Stores the DateTime and Offset)  
      + Column2: nvarchar(35) (Stores the Tzdb DateTimeZone Id)  

## Quick Start ##
### NHibernate ###

Install using nuget.
```Powershell
Install-Package Dematt.Airy.Nhibernate.NodaTime
```

Given the following class to map.
```C#
public class ZonedDateTimeTestEntity
{
    public virtual int Id { get; set; }

    public virtual string Description { get; set; }

    public virtual Instant StartInstant { get; set; }

    public virtual Instant? FinishInstant { get; set; }

    public virtual ZonedDateTime StartZonedDateTime { get; set; }
    
    public virtual ZonedDateTime? FinishZonedDateTime { get; set; }
}
```

The mapping code would look like this.
```C#
var myEntities = new [] {
    typeof(ZonedDateTimeTestEntity)
};

var modelMapper = new ModelMapper();
modelMapper.Class<ZonedDateTimeTestEntity>(c =>
{
    c.Id(p => p.Id, m =>
    {
        m.Generator(Generators.Native);
    });
    c.Property(p => p.Description, m =>
    {
        m.Length(100);
    });
    c.Property(p => p.StartInstant, m =>
    {
        m.Type<InstantType>();
    });
    c.Property(p => p.FinishInstant, m =>
    {
        m.Type<InstantType>();
    });
    c.Property(p => p.StartZonedDateTime, m =>
    {
        m.Type<ZonedDateTimeTzdbType>();
        m.Columns(f => f.Name("StartZonedDateTime"), f => f.Name("StartZoneDateTimeTimeZoneId"));
    });
    c.Property(p => p.FinishZonedDateTime, m =>
    {
        m.Type<ZonedDateTimeTzdbType>();
        m.Columns(f => f.Name("FinishZonedDateTime"), f => f.Name("FinishZoneDateTimeTimeZoneId"));
    });
});

var _configuration = new Configuration();
_configuration.AddMapping(modelMapper.CompileMappingFor(myEntities));

/// Optional add the linq extension to allow querying by ZonedDateTime.ToDateTimeOffset()
_configuration.LinqToHqlGeneratorsRegistry<LinqToHqlGeneratorsRegistry>();

var factory = configuration.BuildSessionFactory();
var session = factory.OpenSession();
```

For other examples please see the tests.

## Acknowledgements ##
Jon Skeet and the other people that work on the NodaTime project.
  + http://nodatime.org/
  + https://github.com/nodatime/nodatime

This stackoverflow post that got me started on this project when I was looking at the best way to store NodaTime structs.
  + http://stackoverflow.com/questions/34452792/using-offsetdatetime-with-nhibernate
  + https://gist.github.com/chilversc/d1ba1fdbae58d8a13704

##Caveats##
As noted by the NodaTime project, dates and times are a complicated and extremely difficult area in which to handle all cases correctly.
Every project will have its own unique requirements and rules as to how to handle them, this is not intended to be a global solution to storing NodaTime structs.
It is inevitably driven be the requirements of the projects I am currently working on.  For example we store the Instant part of a ZonedDateTime as a datetimeoffset and not as a bigint (Int64) because of external reporting requirements of the database where the data is stored.
Having said all of that if it is missing something you require or you have an issue please do not hesitate to raise a github issue or pull request.
