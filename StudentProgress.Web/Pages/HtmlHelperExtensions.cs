using System;
using System.Linq.Expressions;
using HtmlTags;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentProgress.Web.Pages
{
    public static class HtmlHelperExtensions
    {
        public static HtmlTag FormBlock<T, TMember>(this IHtmlHelper<T> helper,
            Expression<Func<T, TMember>> expression,
            Action<HtmlTag> labelModifier = null,
            Action<HtmlTag> inputModifier = null
        ) where T : class
        {
            labelModifier ??= _ => { };
            inputModifier ??= _ => { };

            var divTag = new HtmlTag("div");
            divTag.AddClass("form-floating mb-3");

            var labelTag = helper.Label(expression);
            inputModifier(labelTag);

            var inputTag = helper.Input(expression)
                .AddClass("form-control");
            inputTag.Attr("placeholder", "");
            inputModifier(inputTag);

            var validationTag = helper.ValidationMessage(expression)
                .AddClass("text-danger");

            divTag.Append(inputTag);
            divTag.Append(labelTag);
            divTag.Append(validationTag);

            return divTag;
        } 
    }
}