using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kernel
{
    /// <summary>
    /// Проверка целостности сем. сети, включающая:
    /// 1) конроль рекурсии (отсутствие циклических связей is_a и is_instance)
    /// 2) отсутствие экземпляров экземпляров, т.е. A -#is_instance-> B -#is_instance-> C
    /// 3) контроль метазнаний: имена вершин метазнаний не должны совпадать с именами зарезервированных дуг
    /// 4) из именованной вершины не должно выходить дуг
    /// 5) в именованную вершину может входить только дуга с именем #Name
    /// 6) нельзя, чтобы из вершины выходило несколько дуг с именем #is_instance и #is_a
    /// </summary>
    public class Verification
    {
        #region Переменные

        public List<string> Errors { get; private set; }

        public bool NoErros { get { return Errors.Count == 0; } }

        #endregion

        #region Инициализация
        public Verification()
        {
            Errors = new List<string>();
        } 

        #endregion

        #region Верификация
        /// <summary>
        /// Основная функция, выполняющая все проверки
        /// </summary>
        /// <returns></returns>
        public void Verificate()
        {
            // 1) конроль рекурсии (отсутствие циклических связей is_a и is_instance)
            Errors.AddRange(CheckRecursion());
            // 2) отсутствие экземпляров экземпляров, т.е. A -#is_instance-> B -#is_instance-> C
            Errors.AddRange(CheckInstancesOfInstances());
            // 3) контроль метазнаний: имена вершин метазнаний не должны совпадать с именами зарезервированных дуг
            Errors.AddRange(CheckMetadata());
            // 4) из именованной вершины не должно выходить дуг
            Errors.AddRange(CheckNamedNodes());
            // 5) в именованную вершину может входить только дуга с именем #Name
            Errors.AddRange(CheckArcToNamedNodes());
        } 
        #endregion

        #region Проверка рекурсии
        /// <summary>
        /// Проверка рекурсии
        /// </summary>
        /// <returns>Возвращает список ошибок</returns>
        private IEnumerable<string> CheckRecursion()
        {
            var errors = new List<string>();
            var nodesWithIsA = SemanticWeb.Web().Nodes.Where(x => SemanticWeb.Web().ArcExists(x.ID, "#is_a"));
            foreach (var node in nodesWithIsA)
            {
                foreach (var parentNode in SemanticWeb.Web().GetAllAttr(node.ID, "#is_a"))
                {
                    if (!OldestParentExists(node.ID, parentNode.ID))
                        errors.Add(string.Format("Циклическая зависимость: вершина {0} является "
                                                 + "потомком самой себя", node));
                }
            }
            return errors;
        }

        /// <summary>
        /// Проходит вверх по иерархии по дугам #is_a. Если дошли до системных вершин, то всё ок, 
        /// если снова вернулись на исходную вершину, то есть цикл.
        /// </summary>
        /// <param name="startNodeID">Вершина для проверки</param>
        /// <param name="nodeID">Вершина для проверки</param>
        /// <returns>ID родителя</returns>
        private bool OldestParentExists(int startNodeID, int nodeID)
        {
            //пришли в ту же вершину
            if (startNodeID == nodeID) return false;
            //дошли до конца
            if (!SemanticWeb.Web().ArcExists(nodeID, "#is_a"))
                return true;
            //идём дальше
            var parentNode = SemanticWeb.Web().GetAllAttr(nodeID, "#is_a");
            return parentNode.Select(node => OldestParentExists(startNodeID, node.ID))
                .FirstOrDefault();
        } 
        #endregion

        #region Проверка экземпляров экземпляров
        /// <summary>
        /// Проверка экземпляров экземпляров
        /// </summary>
        /// <returns>Возвращает список ошибок</returns>
        private IEnumerable<string> CheckInstancesOfInstances()
        {
            const string arcName = "#is_instance";
            return (from node in SemanticWeb.Web().Nodes
                    where SemanticWeb.Web().ArcExists(node.ID, arcName)
                    let classNode = SemanticWeb.Web().GetAttr(node.ID, arcName)
                    where SemanticWeb.Web().ArcExists(classNode.ID, arcName)
                    select
                        string.Format("Вершина {0} является экземпляром {1}, " + "являющейся экземпляром {2}", node.Name,
                                      classNode.Name, SemanticWeb.Web().GetAttr(classNode.ID, arcName)));
        } 
        #endregion

        #region Проверка метазнаний
        private IEnumerable<string> CheckMetadata()
        {
            //node = a_part_of smth is_a metadata
            //node.name not in SemanticWeb.AllReservedArcs

            return (from node in SemanticWeb.Web().Nodes.Where(x => SemanticWeb.Web().ArcExists(x.ID, "#a_part_of"))
                    let parent = SemanticWeb.Web().GetAttr(node.ID, "#a_part_of")
                    where
                        SemanticWeb.Web().ArcExists(parent.ID, "#is_a", SemanticWeb.Web().Atom("#Metadata")) &&
                        SemanticWeb.AllReservedArcs.Contains(node.Name.Trim().ToUpper())
                    select "Имя вершины из метазнаний совпадает с именем системной дуги");
        } 
        #endregion

        #region Проверки, связанные с именованными вершинами
        private IEnumerable<string> CheckNamedNodes()
        {
            return (from node in SemanticWeb.Web().Nodes
                    where node.Name != string.Empty && node.Name != "#System" && SemanticWeb.Web().Arcs
                        .Where(x => x.Name.Substring(0,2) != "_#")
                        .Any(x => x.From == node.ID)
                    select string.Format("Из именованной вершины {0} выходят дуги", node));
        } 

        private IEnumerable<string> CheckArcToNamedNodes()
        {
            return (from namedNode in SemanticWeb.Web().Nodes.Where(x => x.Name != string.Empty)
                    from arc in SemanticWeb.Web().Arcs.Where(x => x.To == namedNode.ID)
                    where arc.Name.Trim().ToUpper() != "#Name".ToUpper()
                    select string.Format("В именованную вершину {0} входит дуга с именем {1}", 
                    namedNode.Name, arc.Name));
        }

        #endregion
    }
}
