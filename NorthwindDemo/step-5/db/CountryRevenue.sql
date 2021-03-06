SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE CountryRevenue
	@Country VARCHAR(50),
	@FromDate VARCHAR(50),
	@ToDate VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Orders.OrderDate As Date, (Quantity * UnitPrice) AS Value
	FROM [Order Details]
		INNER JOIN Orders ON Orders.OrderID = [Order Details].OrderID
	WHERE Orders.ShipCountry = @Country AND Orders.OrderDate >=  CONVERT(DATE, @FromDate,112) AND Orders.OrderDate <=  CONVERT(DATE, @ToDate,112)
	GROUP BY (Quantity * UnitPrice) , Orders.OrderDate
END
