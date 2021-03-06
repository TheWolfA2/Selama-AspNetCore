﻿using Guilded.Areas.Forums.Constants;
using Guilded.Areas.Forums.Controllers;
using Guilded.Areas.Forums.ViewModels;
using Guilded.Data.Forums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Guilded.Tests.Areas.Forums.Controllers.ShareControllerTests
{
    public class WhenThreadIsCalled : ShareControllerTest
    {
        private Thread _defaultThread;

        protected override string DefaultShareLink => "https://example.com/forums/threads/";

        protected override Expression<Func<ShareController, Func<int, Task<IActionResult>>>> AsyncActionToTest =>
            c => c.Thread;

        [SetUp]
        public void SetUp()
        {
            _defaultThread = new Thread
            {
                Id = DefaultId,
                Forum = new Forum
                {
                    IsActive = true
                }
            };

            MockUrlHelper.Setup(u => u.RouteUrl(
                It.IsAny<UrlRouteContext>()
            )).Returns(DefaultShareLink);
            MockMarkdownConverter.Setup(md => md.ConvertAndStripHtml(It.IsAny<string>()))
                .Returns(string.Empty);
            MockDataContext.Setup(d => d.GetThreadByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_defaultThread);
        }

        [Test]
        public async Task IfIdIsNotFoundThenNotFoundResultReturned()
        {
            MockDataContext.Setup(d => d.GetThreadByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Thread)null);

            await ThenResultShouldBeNotFound();
        }

        [Test]
        public async Task IfThreadIsDeletedThenNotFoundResultReturned()
        {
            _defaultThread.IsDeleted = true;

            await ThenResultShouldBeNotFound();
        }

        [Test]
        public async Task IfThreadsForumIsInactiveThenNotFoundResultReturned()
        {
            _defaultThread.Forum.IsActive = false;

            await ThenResultShouldBeNotFound();
        }

        [Test]
        public async Task ThenGetThreadByIdAsyncIsCalledWithIdParameter()
        {
            await Controller.Thread(DefaultId);

            MockDataContext.Verify(d => d.GetThreadByIdAsync(It.Is<int>(i => i == DefaultId)));
        }

        [Test]
        public async Task ThenViewModelIsAThreadPreview()
        {
            await ThenViewModelIsOfType<ThreadPreview>();
        }

        [Test]
        public async Task ThenViewModelTitleShouldMatchDataModel()
        {
            const string threadTitle = "My thread title";

            _defaultThread.Title = threadTitle;

            await ThenViewModelTitleMatchesExpected<ThreadPreview>(threadTitle);
        }

        [Test]
        public async Task ThenViewThreadRouteIsUsed()
        {
            await Controller.Thread(DefaultId);

            MockUrlHelper.Verify(u => u.RouteUrl(
                It.Is<UrlRouteContext>(c => 
                    c.Protocol == "https" && 
                    c.RouteName == RouteNames.ViewThreadByIdRoute &&
                    c.Values.GetType().GetProperty("id") != null
                )
            ));
        }

        [Test]
        public async Task ThenShareLinkIsResultOfUrlHelper()
        {
            await ThenViewModelShareLinkMatchesDefaultShareLink();
        }

        [Test]
        public async Task ThenMarkdownConverterShouldUseThreadContent()
        {
            const string threadContent = "I am the content of this thread!";

            _defaultThread.Content = threadContent;

            await Controller.Thread(DefaultId);

            MockMarkdownConverter.Verify(c => c.ConvertAndStripHtml(It.Is<string>(s => s == threadContent)));
        }

        [Test]
        public async Task ThenViewModelDescriptionIsConvertedMarkdownThatIsStrippedOfHtml()
        {
            const string threadContent = "__Yes__ I _am_ markdown!";
            const string strippedContent = "Yes I am markdown!";

            _defaultThread.Content = threadContent;

            MockMarkdownConverter.Setup(c => c.ConvertAndStripHtml(It.IsAny<string>()))
                .Returns(strippedContent);

            var viewModel = await ShareThreadViewModel();

            viewModel.Description.ShouldBe(strippedContent);
        }

        [Test]
        public async Task ThenContentPreviewDoesNotExceedDesiredMaximum()
        {
            var threadContent = new string('a', ShareController.ShareDescriptionLength + 5);

            MockMarkdownConverter.Setup(c => c.ConvertAndStripHtml(It.IsAny<string>()))
                .Returns(threadContent);

            var viewModel = await ShareThreadViewModel();

            viewModel.Description.Length.ShouldBe(ShareController.ShareDescriptionLength);
        }

        private async Task<ThreadPreview> ShareThreadViewModel()
        {
            var result = (ViewResult)await Controller.Thread(DefaultId);
            return result.Model as ThreadPreview;
        }
    }
}
