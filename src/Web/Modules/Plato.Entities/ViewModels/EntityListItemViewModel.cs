﻿using System.Collections.Generic;
using PlatoCore.Layout.Models;
using PlatoCore.Models;

namespace Plato.Entities.ViewModels
{

    public class EntityListItemViewModel<TModel> where TModel : class
    {

        public TModel Entity { get; set; }

        public ILabelBase Category { get; set; }

        public IEnumerable<ILabelBase> Labels { get; set; }

        public IEnumerable<ITagBase> Tags { get; set; }

        public EntityIndexOptions Options { get; set; }

        public TagAlterations TagAlterations { get; set; }

    }

}
