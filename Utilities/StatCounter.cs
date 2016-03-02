/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Net;

namespace ScourgeBloom.Utilities
{
    public static class StatCounter
    {
        public static void StatCount()
        {
            const string url = "http://c.statcounter.com/10723361/0/69115124/0/";
            // Download the file to increment the statcount.
            new WebClient().DownloadDataAsync(new Uri(url));
        }
    }
}