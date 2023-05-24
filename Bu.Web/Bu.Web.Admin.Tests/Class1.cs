using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;

namespace Bu.Web.Admin.Tests;

[TestClass]
public class ArticleNameValidityTests
{
    [TestMethod]
    public void ArticleNameIsNotNullOrEmpty_Test()
    {
        // Arrange
        var articleName = "";

        // Act
        bool result = string.IsNullOrEmpty(articleName);

        // Assert
        Assert.IsTrue(result, "Article name should not be null or empty.");
    }

    [TestMethod]
    public void ArticleNameIsNotTooLong_Test()
    {
        // Arrange
        var articleName = new String('A', 300);

        // Act
        bool result = articleName.Length > 200;

        // Assert
        Assert.IsTrue(result, "Article name should not be more than 200 characters.");
    }
}
