﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Knowzy
{
    public interface IPhotoService
    {
        Task<byte[]> TakePhotoAsync();
    }
}
