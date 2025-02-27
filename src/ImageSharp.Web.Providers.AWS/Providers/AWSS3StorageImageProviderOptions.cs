// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace SixLabors.ImageSharp.Web.Providers.AWS
{
    /// <summary>
    /// Configuration options for the <see cref="AWSS3StorageImageProviderOptions"/> provider.
    /// </summary>
    public class AWSS3StorageImageProviderOptions
    {
        /// <summary>
        /// Gets or sets the collection of blob container client options.
        /// </summary>
        public ICollection<AWSS3BucketClientOptions> S3Buckets { get; set; } = new HashSet<AWSS3BucketClientOptions>();
    }

    /// <summary>
    /// Configuration options for the <see cref="AWSS3StorageImageProvider"/> provider.
    /// </summary>
    public class AWSS3BucketClientOptions : IAWSS3BucketClientOptions
    {
        /// <inheritdoc/>
        public string Region { get; set; }

        /// <inheritdoc/>
        public string BucketName { get; set; }

        /// <inheritdoc/>
        public string AccessKey { get; set; }

        /// <inheritdoc/>
        public string AccessSecret { get; set; }

        /// <inheritdoc/>
        public string Endpoint { get; set; }

        /// <inheritdoc/>
        public bool UseAccelerateEndpoint { get; set; }
    }
}
