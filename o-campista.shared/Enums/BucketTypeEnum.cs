using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace o_campista.shared.Enums
{
    public enum BucketTypeEnum
    {
        [Description("BucketGift")]
        BucketGift = 1,
        [Description("BucketCamping")]
        BucketCamping =2,
        [Description("BucketUser")]
        BucketUser = 3,
    }
}
