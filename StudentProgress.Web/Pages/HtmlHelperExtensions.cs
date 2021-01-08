using System;
using System.Linq.Expressions;
using HtmlTags;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentProgress.Web.Pages
{
    public static class HtmlHelperExtensions
    {
        public static HtmlTag SubmitBlock(this IHtmlHelper helper, string buttonText = "Submit")
        {
             var divTag = new HtmlTag("div");
             divTag.AddClass("form-group");

             var submitTag = new HtmlTag("input");
             submitTag.Attr("type", "submit");
             submitTag.AddClass("btn btn-primary");
             submitTag.Value(buttonText);
             
             divTag.Append(submitTag);
             
             return divTag;
        }
        
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
            inputTag.Attr("placeholder", "placeholder");
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