﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HtmlTags;
using HtmlTags.Conventions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Topsis.Web.Infrastructure.Tags;

namespace Topsis.Web.Pages
{

    public static class HtmlHelperExtensions
    {
        public static HtmlTag DisplayLabel<T, TMember>(this IHtmlHelper<T> helper, Expression<Func<T, TMember>> expression)
            where T : class
        {
            return helper.Tag(expression, nameof(TagConventions.DisplayLabels));
        }

        public static HtmlTag DisplayLabel<T, TMember>(this IHtmlHelper<List<T>> helper, Expression<Func<T, TMember>> expression)
            where T : class
        {
            var library = helper.ViewContext.HttpContext.RequestServices.GetService<HtmlConventionLibrary>();
            var generator = ElementGenerator<T>.For(library, t => helper.ViewContext.HttpContext.RequestServices.GetService(t));
            return generator.TagFor(expression, nameof(TagConventions.DisplayLabels));
        }

        public static HtmlTag ValidationDiv(this IHtmlHelper helper)
        {
            var outerDiv = new HtmlTag("div")
                .Id("validationSummary")
                .AddClass("validation-summary-valid")
                .Data("valmsg-summary", true);

            var ul = new HtmlTag("ul");
            ul.Add("li", li => li.Style("display", "none"));

            outerDiv.Children.Add(ul);

            return outerDiv;
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
            divTag.AddClass("form-group");

            var labelTag = helper.Label(expression);
            labelModifier(labelTag);

            var inputTag = helper.Input(expression);
            inputModifier(inputTag);

            divTag.Append(labelTag);
            divTag.Append(inputTag);

            return divTag;
        }

        public static string GetAntiforgeryToken(this HttpContext httpContext)
        {
            var antiforgery = (IAntiforgery)httpContext.RequestServices.GetService(typeof(IAntiforgery));
            var tokenSet = antiforgery.GetAndStoreTokens(httpContext);
            //string fieldName = tokenSet.FormFieldName;
            string requestToken = tokenSet.RequestToken;
            return requestToken;
        }

        public static string FormatUtcDate(this DateTime datetime, string format = "HH:mm dd/MM/yyyy")
        {
            return datetime.ToLocalTime().ToString(format);
        }
    }
}
