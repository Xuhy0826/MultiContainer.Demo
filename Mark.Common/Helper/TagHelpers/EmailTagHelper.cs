using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mark.Common.Helper.TagHelpers
{
    [HtmlTargetElement("bold")]             //表示在razor中 <bold> 标签代表这个TagHelper
    [HtmlTargetElement(Attributes = "bold")]    //表示在razor中 <XXX bold> 代表这个TagHelper
    //以上两个同时标注则代表的是或，如果下面这种写法则是与了，HTML 元素必须命名为“bold”并具有名为“bold”的属性 (<bold bold />) 才能匹配
    //[HtmlTargetElement("bold", Attributes = "bold")]
    //当然，名称是可以自定义的，[HtmlTargetElement("MyBold")]
    public class BoldTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("bold");
            output.PreContent.SetHtmlContent("<strong>");
            output.PostContent.SetHtmlContent("</strong>");
        }
    }

    [HtmlTargetElement("email", TagStructure = TagStructure.WithoutEndTag)]
    public class EmailTagHelper : TagHelper
    {
        private const string EmailDomain = "163.com";

        // Can be passed via <email mail-to="..." />. 
        // PascalCase gets translated into kebab-case.
        public string MailTo { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string address;
            if (RegexUtil.IsEmail(MailTo))
            {
                address = MailTo;
            }
            else
            {
                address = string.Concat(MailTo, $"@{EmailDomain}");
            }
            output.TagName = "a";    // Replaces <email> with <a> tag
            output.Attributes.SetAttribute("href", "mailto:" + address);
            output.Content.SetContent(address);
        }
    }

    /// <summary>
    /// 条件标记帮助程序在传递 true 值时呈现输出。
    /// </summary>
    [HtmlTargetElement(Attributes = nameof(Condition))]
    public class ConditionTagHelper : TagHelper
    {
        public bool Condition { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Condition)
            {
                output.SuppressOutput();
            }
        }
    }

    [HtmlTargetElement("p")]
    public class AutoLinkerHttpTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();
            // Find Urls in the content and replace them with their anchor tag equivalent.
            output.Content.SetHtmlContent(Regex.Replace(
                childContent.GetContent(),
                @"\b(?:https?://)(\S+)\b",
                "<a target=\"_blank\" href=\"$0\">$0</a>"));  // http link version}
        }
    }
}
