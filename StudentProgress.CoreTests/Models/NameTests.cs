using FluentAssertions;

namespace StudentProgress.CoreTests.Models;

public class NameTests
{
    [Fact]
    public void Creates_a_name()
    {
        var name = Name.Create("Timo");

        name.IsSuccess.Should().BeTrue();
        name.Value.Value.Should().Be("Timo");
    }

    [Fact]
    public void Cannot_create_an_empty_name()
    {
        var name = Name.Create("");

        name.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Gets_first_part_of_name_by_separator()
    {
        var name = Name.Create("S-DB-S2-CMK - S2-DB02 - 2223nj");

        var firstPart = name.Value.GetFirstPart(' ');

        firstPart.Should().Be("S-DB-S2-CMK");
    }

    [Fact]
    public void Gets_entire_name_if_get_part_separator_is_not_in_name()
    {
         var name = Name.Create("S-DB-S2-CMK");

        var firstPart = name.Value.GetFirstPart(' ');

        firstPart.Should().Be("S-DB-S2-CMK");       
    }
}