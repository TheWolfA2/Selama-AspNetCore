﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guilded.Areas.Forums.Controllers;
using Guilded.Data.Forums;
using Guilded.Data.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Guilded.Tests.Areas.Forums.Controllers.ThreadsControllerTests
{
    public class WhenThreadBySlugIsCalled : ThreadsControllerTest
    {
        private const string DefaultThreadSlug = "example-slug";

        private Forum _defaultParentForum;

        private Thread _defaultThread;
        private List<Reply> _defaultReplies;
        private ApplicationUser _defaultAuthor;

        [SetUp]
        public void SetUp()
        {
            _defaultParentForum = new Forum();

            _defaultReplies = new List<Reply>();
            _defaultAuthor = new ApplicationUser
            {
                UserName = "Default user"
            };
            _defaultThread = new Thread
            {
                Slug = DefaultThreadSlug,
                Replies = _defaultReplies,
                Author = _defaultAuthor,
                Forum = _defaultParentForum
            };

            MockDataContext.Setup(db => db.GetThreadBySlugAsync(It.IsAny<string>()))
                .ReturnsAsync(_defaultThread);
        }

        [Test]
        public async Task IfPageIsLessThanOrEqualToZeroThenRedirectToSelf([Values(
            -2,
            -1,
            0)] int page)
        {
            var result = await Controller.ThreadBySlug(DefaultThreadSlug, page) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo(nameof(ThreadsController.ThreadBySlug)));
            Assert.That(result.RouteValues.ContainsKey("slug"), Is.True);
            Assert.That(result.RouteValues["slug"], Is.EqualTo(DefaultThreadSlug));
        }

        [Test]
        public async Task IfThreadIsNotFoundThenNotFoundResultReturned()
        {
            MockDataContext.Setup(db => db.GetThreadBySlugAsync(It.IsAny<string>()))
                .ReturnsAsync((Thread)null);

            var result = await Controller.ThreadBySlug(DefaultThreadSlug) as NotFoundResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task ThenGetThreadBySlugAsyncIsCalled()
        {
            await Controller.ThreadBySlug(DefaultThreadSlug);

            MockDataContext.Verify(db => db.GetThreadBySlugAsync(It.Is<string>(s => s == DefaultThreadSlug)));
        }
    }
}
