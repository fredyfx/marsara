﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RC.UI;
using RC.App.BizLogic.PublicInterfaces;
using RC.App.PresLogic.Controls;

namespace RC.App.PresLogic
{
    /// <summary>
    /// The sprite-group of the isometric tiles.
    /// </summary>
    class IsoTileSpriteGroup : SpriteGroup
    {
        /// <summary>
        /// Constructs an IsoTileSpriteGroup instance.
        /// </summary>
        /// <param name="tilesetView">Reference to a view on the tileset.</param>
        public IsoTileSpriteGroup(ITileSetView tilesetView)
            : base()
        {
            if (tilesetView == null) { throw new ArgumentNullException("tilesetView"); }
            this.tilesetView = tilesetView;
        }

        /// <see cref="SpriteGroup.Load_i"/>
        protected override List<UISprite> Load_i()
        {
            List<UISprite> retList = new List<UISprite>();
            foreach (MapSpriteType tileType in this.tilesetView.GetIsoTileTypes())
            {
                UISprite tile = UIRoot.Instance.GraphicsPlatform.SpriteManager.LoadSprite(tileType.ImageData, UIWorkspace.Instance.PixelScaling);
                tile.TransparentColor = tileType.TransparentColorStr != null ?
                                        UIResourceLoader.LoadColor(tileType.TransparentColorStr) :
                                        RCMapDisplayBasic.DEFAULT_TILE_TRANSPARENT_COLOR;
                tile.Upload();
                retList.Add(tile);
            }
            return retList;
        }

        /// <summary>
        /// Reference to a view on the tileset.
        /// </summary>
        private ITileSetView tilesetView;
    }
}