using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.EnemySkill
{
    internal class Main : ControlMenu, ISelectorUser
    {
        private const int COLUMNS = 2;
        
        #region Layout
        
        const int x1 = 50;
        const int xs = 250;
        const int ys = 35;
        const int cx = -15;
        const int cy = -8;
        
        #endregion Layout
        
        private int _xopt;
        private int _yopt;
        private int _topRow;
        private int _totalRows = (EnemySkillMateria.TOTAL_ENEMY_SKILLS / COLUMNS) + ((EnemySkillMateria.TOTAL_ENEMY_SKILLS % COLUMNS == 0) ? 0 : 1);
        private readonly int _visibleRows = 3;
        
        /// <summary>Spells: row, column.</summary>
        private Spell[,] _spells;
        
        public Main(EnemySkillMateria esm)
            : base(
                5,
                Config.Instance.WindowHeight * 7 / 10 + 20,
                Config.Instance.WindowWidth * 3 / 4,
                (Config.Instance.WindowHeight * 5 / 20) - 25)
        {
            _spells = new Spell[_totalRows, COLUMNS];
            
            for (int i = 0; i < EnemySkillMateria.TOTAL_ENEMY_SKILLS; i++)
                if (((esm.AP >> i) & 1) > 0)
            {
                Spell s = esm.EnemySkills[i];
                _spells[s.Order / COLUMNS, s.Order % COLUMNS] = s;
            }
            
            Reset();
        }
        
        
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (_yopt > 0) _yopt--;
                    if (_topRow > _yopt) _topRow--;
                    break;
                case Key.Down:
                    if (_yopt < _totalRows - 1) _yopt++;
                    if (_topRow < _yopt - _visibleRows + 1) _topRow++;
                    break;
                case Key.Left:
                    if (_xopt > 0) _xopt--;
                    break;
                case Key.Right:
                    if (_xopt < COLUMNS - 1) _xopt++;
                    break;
                case Key.X:
                    Seven.BattleState.Screen.PopControl();
                    Reset();
                    break;
                case Key.Circle:
                    if (!String.IsNullOrEmpty(_spells[_yopt, _xopt].ID))
                    {
                        Spell skill = _spells[_yopt, _xopt];

                        if (Seven.BattleState.Commanding.MP >= skill.MPCost)
                        {
                            Seven.BattleState.Screen.ActivateSelector(skill.Target, skill.TargetEnemiesFirst);
                        }
                    }
                    break;
            }
        }
        public bool ActOnSelection(IEnumerable<Combatant> targets)
        {
            Seven.BattleState.Commanding.UseMP(_spells[_yopt, _xopt].MPCost);
            
//            AbilityState state;
//            
//            state = (AbilityState)Seven.BattleState.Commanding.Ability.Clone();
//            
//            state.Performer = Seven.BattleState.Commanding;
//            
//            state.Action += delegate() { _spells[_xopt, _yopt].Action(); };

            return true;
        }


        
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            
            int j = Math.Min(_visibleRows + _topRow, _totalRows);
            
            for (int b = _topRow; b < j; b++)
                for (int a = 0; a < COLUMNS; a++)
                    Text.ShadowedText(g, String.IsNullOrEmpty(_spells[b, a].ID) ? "" : _spells[b, a].Name,
                                          X + x1 + a * xs,
                                          Y + (b - _topRow + 1) * ys);
            
            
            if (IsControl)
                Shapes.RenderCursor(g,
                                      X + x1 + cx + _xopt * xs,
                                      Y + cy + (_yopt - _topRow + 1) * ys);
            
            Seven.BattleState.Screen.EnemySkillInfo.Draw(d);
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void Reset()
        {
            _topRow = 0;
            Visible = false;
            Seven.BattleState.Screen.EnemySkillInfo.Visible = false;
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
            Seven.BattleState.Screen.EnemySkillInfo.Visible = true;
        }
        
        public bool IsValid { get { return _totalRows > 0; } }
        
        public override string Info
        { get { return String.IsNullOrEmpty(_spells[_yopt, _xopt].ID) ? "" : _spells[_yopt, _xopt].Desc; } }
        public Spell Selected { get { return _spells[_yopt, _xopt]; } }
    }
}

