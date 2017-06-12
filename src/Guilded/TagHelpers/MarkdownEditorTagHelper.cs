﻿using Guilded.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Guilded.TagHelpers
{
    [HtmlTargetElement("markdown-editor", ParentTag = "form", TagStructure = TagStructure.WithoutEndTag)]
    public class MarkdownEditorTagHelper : TagHelper
    {
        private string _content;
        public string Content
        {
            get =>_content ??
                (_content = (Context.ViewData.Model as IMarkdownContent).Content);
            set =>_content = value;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext Context { get; set; }

        private readonly IHtmlHelper _htmlHelper;

        public MarkdownEditorTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            (_htmlHelper as IViewContextAware).Contextualize(Context);

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
            output.Content.SetHtmlContent(await _htmlHelper.PartialAsync("MarkdownEditor", new MarkdownContent { Content = Content }));
        }
    }
}
