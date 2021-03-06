﻿using Microsoft.Extensions.Logging;
using System;

namespace Vision.Core
{
    public class RepositoryMatcher : IRepositoryMatcher
    {
        private readonly ILogger<RepositoryMatcher> logger;

        public RepositoryMatcher(ILogger<RepositoryMatcher> logger)
        {
            this.logger = logger;
        }

        public bool IsMatch(string url1, string url2)
        {
            try
            {
                Uri uri1 = new Uri(url1.Replace("scm/", string.Empty));
                Uri uri2 = new Uri(url2.Replace("scm/", string.Empty));

                bool sameHost = Uri.Compare(uri1, uri2, UriComponents.Host, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
                bool samePath = Uri.Compare(uri1, uri2, UriComponents.Path, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;

                return sameHost && samePath;
            }
            catch (NullReferenceException e)
            {
                logger.LogError(e, $"Error making comparison with repositories");
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Could not compare {url1} and {url2}");
            }

            return false;
        }
    }
}
