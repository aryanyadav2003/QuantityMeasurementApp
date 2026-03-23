IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='quantity_measurement_entity' AND xtype='U')
CREATE TABLE quantity_measurement_entity (
    id                  INT IDENTITY(1,1) PRIMARY KEY,
    operand1_value      FLOAT           NOT NULL,
    operand1_unit       NVARCHAR(50)    NOT NULL,
    operand1_type       NVARCHAR(50)    NOT NULL,
    operand2_value      FLOAT           NULL,
    operand2_unit       NVARCHAR(50)    NULL,
    operand2_type       NVARCHAR(50)    NULL,
    operation           NVARCHAR(20)    NOT NULL,
    result_value        FLOAT           NULL,
    result_unit         NVARCHAR(50)    NULL,
    result_type         NVARCHAR(50)    NULL,
    comparison_result   BIT             NULL,
    scalar_result       FLOAT           NULL,
    has_error           BIT             NOT NULL DEFAULT 0,
    error_message       NVARCHAR(500)   NULL,
    timestamp           DATETIME2       NOT NULL DEFAULT GETDATE()
)