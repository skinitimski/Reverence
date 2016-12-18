using System;
using System.Linq;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset.Materia;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Materia
{      
    internal sealed class Stats : Menu.Menu
    {
        #region Layout
        
        const int x0 = 10; // first column
        const int x2 = 60; // second column
        const int x1 = x2 - 20; // orb column
        const int x4 = 300; // equip eff
        const int x5 = x4 + 30; // ap
        const int x8 = 485; // end
        const int x7 = x8 - 15; // end
        const int x6 = x8 - 35; // star-slash
        
        const int y0 = 40; // top row
        const int yp = y0 - 7; // orb row
        const int y1 = y0 + 30; // AP
        const int y2 = y1 + 27; // nxt lvl
        const int y3 = y2 + 40; // titles
        const int y4 = y3 + 30; // first ability row
        const int ys = 27;
        
        
#endregion
        
        public Stats(SevenMenuState menuState, ScreenState screenState)
            : base(
                2,
                screenState.Height * 5 / 12,
                screenState.Width * 5 / 8,
                screenState.Height * 8 / 15)
        {
            MenuState = menuState;
        }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            MateriaOrb orb = MenuState.MateriaTop.Selection;

            if (orb == null)
            {
                orb = MenuState.MateriaList.Selection;
            }
            
            
            if (orb != null)
            {
                Shapes.RenderCircle(g, Colors.WHITE, 9, X + x1, Y + yp);
                Shapes.RenderCircle(g, orb.Color, 7, X + x1, Y + yp);
                Text.ShadowedText(g, orb.Name, X + x2, Y + y0);
                
                Cairo.Color greenish = Colors.TEXT_TEAL;
                Cairo.Color yellow = Colors.TEXT_YELLOW;
                Cairo.Color red = Colors.TEXT_RED;
                                
                if (orb is EnemySkillMateria)
                {
                    EnemySkillMateria esm = (EnemySkillMateria)orb;
                    
                    string mask = "";
                    for (int i = 0; i < MenuState.Seven.Data.EnemySkillCount; i++)
                    {
                        mask += ((esm.AP >> i) & 1) > 0 ? "1" : "0";
                    }
                    Text.ShadowedText(g, mask, X + x1, Y + y3);
                }
                else
                {
                    string lvl = (orb.Level + 1).ToString() + "/";
                    te = g.TextExtents(lvl);
                    Text.ShadowedText(g, lvl, X + x6 - te.Width, Y + y0);
                    
                    string lvls = orb.Tiers.Length.ToString();
                    te = g.TextExtents(lvls);
                    Text.ShadowedText(g, lvls, X + x7 - te.Width, Y + y0);
                    
                    te = g.TextExtents("Level");
                    Text.ShadowedText(g, greenish, "Level", X + x5 - te.Width, Y + y0);
                    
                    
                    string ap;
                    if (orb.AP < orb.Tiers[orb.Tiers.Length - 1])
                    {
                        ap = orb.AP.ToString();
                    }
                    else
                    {
                        ap = "MASTER";
                    }
                    te = g.TextExtents(ap);
                    Text.ShadowedText(g, ap, X + x7 - te.Width, Y + y1);
                    te = g.TextExtents("AP");
                    Text.ShadowedText(g, greenish, "AP", X + x5 - te.Width, Y + y1);
                    
                    
                    string nxt;
                    if (orb.Master)
                    {
                        nxt = "0";
                    }
                    else
                    {
                        nxt = (orb.Tiers[orb.Level + 1] - orb.AP).ToString();
                    }
                    te = g.TextExtents(nxt);
                    Text.ShadowedText(g, nxt, X + x7 - te.Width, Y + y2);
                    te = g.TextExtents("To next level");
                    Text.ShadowedText(g, greenish, "To next level", X + x5 - te.Width, Y + y2);
                    
                    
                    Text.ShadowedText(g, greenish, "Ability List", X + x0, Y + y3);
                    
                    int k = 0;
                    foreach (string s in orb.AbilityDescriptions)
                    {
                        if (!String.IsNullOrEmpty(s))
                        {
                            Color abilityTextColor = Colors.GRAY_4;

                            if (orb is MasterMateria || orb.Abilities.Contains(s))
                            {
                                abilityTextColor = Colors.WHITE;
                            }
                            
                            Text.ShadowedText(g, abilityTextColor, s, X + x1, Y + y4 + (ys * k));

                            k++;
                        }
                    }
                    
                    Text.ShadowedText(g, greenish, "Equip Effect", X + x4, Y + y3);
                    
                    int i = 0;
                    string stat;
                    
                    te = g.TextExtents("-");
                    double dashw = te.Width;
                    
                    te = g.TextExtents("+");
                    double plusw = te.Width;
                    
                    #region Strength
                    if (orb.StrengthMod != 0)
                    {
                        Text.ShadowedText(g, "Str", X + x5, Y + y4 + (ys * i));
                        
                        stat = Math.Abs(orb.StrengthMod).ToString();
                        if (stat.Length < 2)
                        {
                            stat = "0" + stat;
                        }
                        
                        te = g.TextExtents(stat);
                        if (orb.StrengthMod < 0)
                        {
                            Text.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Text.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Strength
                    #region Vitality
                    if (orb.VitalityMod != 0)
                    {
                        Text.ShadowedText(g, "Vit", X + x5, Y + y4 + (ys * i));
                        
                        stat = Math.Abs(orb.VitalityMod).ToString();
                        if (stat.Length < 2)
                        {
                            stat = "0" + stat;
                        }
                        
                        te = g.TextExtents(stat);
                        if (orb.VitalityMod < 0)
                        {
                            Text.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Text.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Vitality
                    #region Dexterity
                    if (orb.DexterityMod != 0)
                    {
                        Text.ShadowedText(g, "Dex", X + x5, Y + y4 + (ys * i));
                        
                        stat = Math.Abs(orb.DexterityMod).ToString();
                        if (stat.Length < 2)
                        {
                            stat = "0" + stat;
                        }
                        
                        te = g.TextExtents(stat);
                        if (orb.DexterityMod < 0)
                        {
                            Text.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Text.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Dexterity
                    #region Magic
                    if (orb.MagicMod != 0)
                    {
                        Text.ShadowedText(g, "Mag", X + x5, Y + y4 + (ys * i));
                        
                        stat = Math.Abs(orb.MagicMod).ToString();
                        if (stat.Length < 2)
                        {
                            stat = "0" + stat;
                        }
                        
                        te = g.TextExtents(stat);
                        if (orb.MagicMod < 0)
                        {
                            Text.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Text.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Magic
                    #region Spirit
                    if (orb.SpiritMod != 0)
                    {
                        Text.ShadowedText(g, "Spr", X + x5, Y + y4 + (ys * i));
                        
                        stat = Math.Abs(orb.SpiritMod).ToString();
                        if (stat.Length < 2)
                        {
                            stat = "0" + stat;
                        }
                        
                        te = g.TextExtents(stat);
                        if (orb.SpiritMod < 0)
                        {
                            Text.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Text.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Spirit
                    #region Luck
                    if (orb.LuckMod != 0)
                    {
                        Text.ShadowedText(g, "Lck", X + x5, Y + y4 + (ys * i));
                        
                        stat = Math.Abs(orb.LuckMod).ToString();
                        if (stat.Length < 2)
                        {
                            stat = "0" + stat;
                        }
                        
                        te = g.TextExtents(stat);
                        if (orb.LuckMod < 0)
                        {
                            Text.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Text.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Luck
                    #region HP
                    if (orb.HPMod != 0)
                    {
                        Text.ShadowedText(g, "MaxHP", X + x5, Y + y4 + (ys * i));
                        
                        stat = Math.Abs(orb.HPMod).ToString();
                        if (stat.Length < 2)
                        {
                            stat = "0" + stat;
                        }
                        stat = stat + "%";
                        te = g.TextExtents(stat);
                        if (orb.HPMod < 0)
                        {
                            Text.ShadowedText(g, "-", X + x8 - te.Width - dashw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, red, stat, X + x8 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Text.ShadowedText(g, "+", X + x8 - te.Width - plusw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, yellow, stat, X + x8 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion HP
                    #region MP
                    if (orb.MPMod != 0)
                    {
                        Text.ShadowedText(g, "MaxMP", X + x5, Y + y4 + (ys * i));
                        
                        stat = Math.Abs(orb.MPMod).ToString();
                        if (stat.Length < 2)
                        {
                            stat = "0" + stat;
                        }
                        stat = stat + "%";
                        te = g.TextExtents(stat);
                        if (orb.MPMod < 0)
                        {
                            Text.ShadowedText(g, "-", X + x8 - te.Width - dashw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, red, stat, X + x8 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Text.ShadowedText(g, "+", X + x8 - te.Width - plusw, Y + y4 + (ys * i));
                            Text.ShadowedText(g, yellow, stat, X + x8 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion MP
                }
            }
        }
        
        private SevenMenuState MenuState { get; set; }
    }
    

}

