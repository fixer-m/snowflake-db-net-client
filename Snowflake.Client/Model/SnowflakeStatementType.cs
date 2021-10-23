namespace Snowflake.Client.Model
{
    public enum SnowflakeStatementType
    {
        UNKNOWN = 0x0000,
        SELECT = 0x1000,

        DML = 0x3000,
        INSERT = 0x3000 + 0x100,
        UPDATE = 0x3000 + 0x200,
        DELETE = 0x3000 + 0x300,
        MERGE = 0x3000 + 0x400,
        MULTI_INSERT = 0x3000 + 0x500,
        COPY = 0x3000 + 0x600,
        COPY_UNLOAD = 0x3000 + 0x700,

        SCL = 0x4000,
        ALTER_SESSION = 0x4000 + 0x100,
        USE = 0x4000 + 0x300,
        USE_DATABASE = 0x4000 + 0x300 + 0x10,
        USE_SCHEMA = 0x4000 + 0x300 + 0x20,
        USE_WAREHOUSE = 0x4000 + 0x300 + 0x30,
        SHOW = 0x4000 + 0x400,
        DESCRIBE = 0x4000 + 0x500,

        TCL = 0x5000,
        DDL = 0x6000
    }
}