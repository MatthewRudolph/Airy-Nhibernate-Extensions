# Dematt.Airy.Nhibernate.NodaTime #

## About ##
NHibenrate Custom UserType and Custom CompositeUserType implementations of the NodaTime structs.
Allows the use of the NodaTime structs in Domain or POCO objects when using NHiberate for data access (ORM).

## Features ##
### Supported Databases: ###
  * All databases that support the datetimeoffset data type should work.
  * Tested with Microsoft SQL Server 2012 but should work for:
    * Microsoft SQL Server 2008
    * Microsoft SQL Server 2008 R2
    * Microsoft SQL Server 2012
    * Microsoft SQL Server 2014

### Supported NodaTime Types: ###

    #### DateTimeZone ####
      Implemented by DateTimeZoneTzdbType stored as a nvarchar(35)
    
    #### Instant ####
      Implemented by InstantType stored as a bigint
    
    #### OffsetDateTime ####
        Implemented by OffsetDateTimeType stored as a datetimeoffset(7)
    
    #### ZonedDateTime ####
        + Implemented by ZonedDateTimeBclType stored as:
            + Column1: datetimeoffset(7) (Stores the DateTime and Offset)
            + Column2: nvarchar(35) (Stores the Bcl DateTimeZone Id)
        + Implemented by ZonedDateTimeTzdbType stored as:
            + Column1: datetimeoffset(7) (Stores the DateTime and Offset)
            + Column2: nvarchar(35) (string) (Stores the Tzdb DateTimeZone Id)