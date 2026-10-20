using BudgetCalculator.Controllers;
using BudgetCalculator.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCalculator.Tests;

public class BudgetControllerTests
{
    [Fact]
    public void Calculate_WithoutInsurance_ReturnsOk500()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest();
        request.DailyCost = 100;
        request.Days = 5;
        request.IncludeInsurance = false;

        var result = controller.Calculate(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(500m, GetTotalAmount(okResult));
    }

    [Fact]
    public void Calculate_WithInsurance_Adds5Percent()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest();
        request.DailyCost = 100;
        request.Days = 5;
        request.IncludeInsurance = true;

        var result = controller.Calculate(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(525m, GetTotalAmount(okResult));
    }

    [Fact]
    public void Calculate_NegativeDays_ReturnsBadRequest()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest();
        request.DailyCost = 100;
        request.Days = -1;
        request.IncludeInsurance = false;

        var result = controller.Calculate(request);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badResult.StatusCode);
    }

    [Fact]
    public void Calculate_ZeroDailyCost_ReturnsZero()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest();
        request.DailyCost = 0;
        request.Days = 5;
        request.IncludeInsurance = false;

        var result = controller.Calculate(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(0m, GetTotalAmount(okResult));
    }

    [Fact]
    public void Calculate_DecimalDailyCost_ReturnsExactKopecks()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest();
        request.DailyCost = 100.50m;
        request.Days = 3;
        request.IncludeInsurance = false;

        var result = controller.Calculate(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(301.50m, GetTotalAmount(okResult));
    }

    private decimal GetTotalAmount(OkObjectResult result)
    {
        var value = result.Value;
        var property = value.GetType().GetProperty("TotalAmount");
        return (decimal)property.GetValue(value);
    }
}
