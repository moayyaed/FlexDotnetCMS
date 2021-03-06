﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkLibrary
{
    public class FieldsMapper : BaseMapper
    {
        private const string MapperKey = "FieldsMapperKey";

        /*public static IEnumerable<Field> GetAll()
        {
            return GetDataModel().Fields.OrderByDescending(b => b.DateCreated);
        }*/

        public static Field GetByID(long id)
        {
            //var allItems = GetAll();
            return BaseMapper.GetDataModel().Fields.FirstOrDefault(item => item.ID == id);
        }

        public static Return Update(IField obj)
        {
            obj.DateLastModified = DateTime.Now;

            return Update(MapperKey, obj);
        }
    }
}