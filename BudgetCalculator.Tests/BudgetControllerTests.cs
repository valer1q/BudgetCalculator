using BudgetCalculator.Controllers;
using BudgetCalculator.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCalculator.Tests;

public class BudgetControllerTests
{
    // Позитивный сценарий: 100 * 5 дней без страховки = 500, статус 200
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

    // Страховка добавляет 5%: 500 + 5% = 525 (с багом возвращалось 2500)
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

    // Отрицательное количество дней должно вернуть 400 Bad Request
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

    // Нулевая стоимость дня допустима, сумма должна быть 0
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

    // Дробная цена: 100.50 * 3 = 301.50 ровно, копейки не теряются
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

    // Достаёт TotalAmount из анонимного объекта ответа через рефлексию
    private decimal GetTotalAmount(OkObjectResult result)
    {
        var value = result.Value;
        var property = value.GetType().GetProperty("TotalAmount");
        return (decimal)property.GetValue(value);
    }
}
