SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE CountryCustomers
	@Country VARCHAR(50),
	@FromDate VARCHAR(50),
	@ToDate VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT CAST(COUNT(Data.Customers) AS decimal) AS Value, Data.Date FROM (
		SELECT 1 AS Customers, CAST(CONVERT(VARCHAR, DATEPART(YEAR, Orders.OrderDate)) + '-' + CONVERT(VARCHAR, DATEPART(MONTH, Orders.OrderDate)) + '-1'  AS DATETIME) AS Date
		FROM Orders
		WHERE Orders.ShipCountry = @Country AND Orders.OrderDate >=  CONVERT(DATE, @FromDate,112) AND Orders.OrderDate <=  CONVERT(DATE, @ToDate,112)
		GROUP BY Orders.CustomerID, CAST(CONVERT(VARCHAR, DATEPART(YEAR, Orders.OrderDate)) + '-' + CONVERT(VARCHAR, DATEPART(MONTH, Orders.OrderDate)) + '-1'  AS DATETIME)
	) AS Data 
	GROUP BY Data.Date
END
