// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace SixLabors.ImageSharp.Web
{
    /// <summary>
    /// Provides a common interface for AWS S3 Bucket Client Options.
    /// </summary>
    internal interface IAWSS3BucketClientOptions
    {
        /// <summary>
        /// Gets or sets the AWS region endpoint (us-east-1/us-west-1/ap-southeast-2).
        /// </summary>
        string Region { get; set; }

        /// <summary>
        /// Gets or sets the AWS bucket name.
        /// </summary>
        string BucketName { get; set; }

        /// <summary>
        /// Gets or sets the AWS key - Can be used to override keys provided by the environment.
        /// If deploying inside an EC2 instance AWS keys will already be available via environment
        /// variables and don't need to be specified. Follow AWS best security practices on  <see href="https://docs.aws.amazon.com/general/latest/gr/aws-access-keys-best-practices.html"/>.
        /// </summary>
        string AccessKey { get; set; }

        /// <summary>
        /// Gets or sets the AWS secret - Can be used to override keys provided by the environment.
        /// If deploying inside an EC2 instance AWS keys will already be available via environment
        /// variables and don't need to be specified. Follow AWS best security practices on  <see href="https://docs.aws.amazon.com/general/latest/gr/aws-access-keys-best-practices.html"/>.
        /// </summary>
        string AccessSecret { get; set; }

        /// <summary>
        /// Gets or sets the AWS endpoint - used for testing to over region endpoint allowing it
        /// to be set to localhost.
        /// </summary>
        string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the S3 accelerate endpoint is used.
        /// The feature must be enabled on the bucket. Follow AWS instruction on <see href="https://docs.aws.amazon.com/AmazonS3/latest/userguide/transfer-acceleration.html"/>.
        /// </summary>
        bool UseAccelerateEndpoint { get; set; }
    }
}
