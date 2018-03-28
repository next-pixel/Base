﻿// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using System.IO;
using ExtCore.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class SqlHelper
    {
        private readonly IStorage _storage;

        public SqlHelper(IStorage storage_)
        {
            _storage = storage_;
        }

        /// <summary>
        /// Execute SQL code from an embedded resource SQL file.
        /// </summary>
        /// <param name="resourcePath_"></param>
        /// <returns></returns>
        public string ExecuteSqlResource(string resourcePath_)
        {
            // TODO
            return "not implemented";
        }

        /// <summary>
        /// Execute SQL code from a plain SQL file.
        /// </summary>
        /// <param name="filePath_"></param>
        /// <returns>Any error information, else null when no error happened</returns>
        public string ExecuteSqlFile(string filePath_)
        {
            if (!File.Exists(filePath_))
            {
                return $"File {filePath_} not found";
            }

            try
            {
                ((DbContext)_storage.StorageContext).Database.ExecuteSqlCommand(File.ReadAllText(filePath_));
            }
            catch (Exception e)
            {
                return $"Error executing SQL: {e.Message} - {e.StackTrace}";
            }
            return null;
        }

    }
}
