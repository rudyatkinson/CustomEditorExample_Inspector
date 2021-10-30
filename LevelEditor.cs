using UnityEditor;
using UnityEngine;

namespace Granzwelt.Editor
{
    [CustomEditor(typeof(Level), true)]
    // Birden fazla objeyi seçip hepsinde bulunan Level
    // property ayarlarını aynı anda değiştirmeyi sağlar.
    [CanEditMultipleObjects]
    public class LevelEditor : UnityEditor.Editor
    {
        private SerializedProperty _enemyCountSerializedProperty;
        private SerializedProperty _difficultSerializedProperty;
        private SerializedProperty _endLevelBossSerializedProperty;
        private SerializedProperty _bossWeaknessSerializedProperty;
        private SerializedProperty _isBossEnabledSerializedProperty;

#region GUIStyle

        // GUIStyle neredeyse tüm GUI elementlerinde kullanılan
        // ve kullanıldığı elementin tarzını değiştirebildiğiniz
        // bir objedir. Buradaki kullanım amacım ise title olarak
        // belirlediğim label text fontunu bold ve kırmızı yaparak
        // ortalamaktır.
        private GUIStyle _headerGUIStyle;

#endregion

#region Width & Heights
        
        private int _spaceBetweenMainAndBossSettings;
        private int _labelEnemyCountWidth;
        private int _labelDifficultMaxWidth;
        private int _eNumDifficultMinWidth;
        private int _labelBossWidth;
        private int _labelWeaknessWidth;
        private int _eNumWeaknessMinWidth;

#endregion

#region Texts

        private string _labelLevelHeaderText;
        private string _labelEnemyCountText;
        private string _labelDifficultText;
        private string _toggleGroupBossText;
        private string _labelBossHeaderText;
        private string _labelBossText;
        private string _propertyBossText;
        private string _labelWeaknessText;
        private string _helpBoxText;

#endregion
        

        // OnEnable içerisinde almak istediğimiz propertyleri alıyoruz.
        private void OnEnable()
        {
#region SerializedProperty Declarations

            _endLevelBossSerializedProperty = serializedObject.FindProperty("_endLevelBoss");
            _difficultSerializedProperty = serializedObject.FindProperty("_difficult");
            _bossWeaknessSerializedProperty = serializedObject.FindProperty("_bossWeakness");
            _enemyCountSerializedProperty = serializedObject.FindProperty("_enemyCount");
            _isBossEnabledSerializedProperty = serializedObject.FindProperty("_isBossEnabled");

#endregion
            
            // _difficult bir enumdur ancak şuan SerializedProperty olarak gözüküyor.
            // İstersek SerializedProperty'den Enum'a çevirebiliriz ancak, inspectorda
            // yapılan değişiklikler Level'daki _difficult'ı etkilemez. Bu yüzden
            // SerializedProperty olarak kalmalıdır, fakat enum'a çevirmeden
            // enum gibi davranmasını sağlayabiliriz. SerializedProperty'nin sahip 
            // olduğu intValue sayesinde enumdaki hangi değerin seçildiğini değiştirebilir, 
            // ya da okuyabiliriz. Burada da enum değerini "0" olarak değiştirmeseydik,
            // başlangıçta enum kısmı boş gözükecekti.
            _difficultSerializedProperty.intValue = 0;
            _bossWeaknessSerializedProperty.intValue = 0;
            
            _headerGUIStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState() {textColor = Color.red}
            };

#region Width Declarations

            _spaceBetweenMainAndBossSettings = 20;
            _labelEnemyCountWidth = 80;
            _labelDifficultMaxWidth = 50;
            _eNumDifficultMinWidth = 50;
            _labelBossWidth = 35;
            _labelWeaknessWidth = 70;
            _eNumWeaknessMinWidth = 40;

#endregion

#region Text Declarations

            _labelLevelHeaderText = "Level Settings";
            _labelEnemyCountText = "EnemyCount";
            _labelDifficultText = "Difficult: ";
            _toggleGroupBossText = "Boss Activated: ";
            _labelBossHeaderText = "Boss Settings";
            _labelBossText = "Boss:";
            _propertyBossText = "";
            _labelWeaknessText = "Weakness: ";
            _helpBoxText = "Boss ayarları bölüm sonunda çıkacak boss için girilmelidir, boş bırakılırsa boss çıkmaz.";

#endregion
        }

        public override void OnInspectorGUI()
        {
            // Update methodu inspectorda göstermek için aldığımız
            // SerializedProperty'leri kopyalayıp değiştirmemizi sağlar.
            // Böylece değişen her SerializedObject, her framade 
            // güncellenir. Bu da run-time sırasında değerlerin
            // değiştiğinde inspector'da da değişmesini sağlar.
            // OnInspectorGUI methodunun başında çalıştırılabilir.
            serializedObject.Update();

            #region LevelSettings
            EditorGUILayout.LabelField(_labelLevelHeaderText, 
                _headerGUIStyle);
            
            EditorGUILayout.BeginHorizontal();
            
            // GUILayout ayarları elementlerin genişlik ve yükseklik
            // ölçülerini özelleştirmenize yarar. Sabit yükseklik ya da
            // genişlik verebildiğiniz gibi min ve max olarak da 
            // ayarlayabilirsiniz.
            EditorGUILayout.LabelField(_labelEnemyCountText,
                GUILayout.Width(_labelEnemyCountWidth));
            _enemyCountSerializedProperty.intValue = EditorGUILayout.IntField(_enemyCountSerializedProperty.intValue);

            EditorGUILayout.LabelField(_labelDifficultText, 
                GUILayout.MaxWidth(_labelDifficultMaxWidth));
            _difficultSerializedProperty.intValue = (int)(LevelDifficult) EditorGUILayout.EnumPopup(
                (LevelDifficult)_difficultSerializedProperty.intValue,
                GUILayout.MinWidth(_eNumDifficultMinWidth));

            EditorGUILayout.EndHorizontal();
            
#endregion

            // 20 pixel boşluk bırakır.
            EditorGUILayout.Space(_spaceBetweenMainAndBossSettings);

#region Level Boss Settings

            // Boss'un etkinleştirilmesi durumunda inspectorda boss
            // ayarları etkinleştirilir.
            _isBossEnabledSerializedProperty.boolValue = EditorGUILayout.BeginToggleGroup(
                _toggleGroupBossText, 
                _isBossEnabledSerializedProperty.boolValue);

            EditorGUILayout.LabelField(_labelBossHeaderText, 
                _headerGUIStyle);
        
            EditorGUILayout.BeginHorizontal();
        
            // Boss property için label oluşturmak yerine, PropertyField'ın
            // ikinci parametresine string girerek oluşturduğum label ile
            // aynı işi yapabilirdim ancak, estetik olarak daha iyi
            // çalışmak istiyorsanız PropertyField'ın ikinci parametresine
            // boş string verebilir, PropertyField'dan önce bir label
            // oluşturabilirsiniz.
            EditorGUILayout.LabelField(_labelBossText, 
                GUILayout.Width(_labelBossWidth));
            EditorGUILayout.PropertyField(_endLevelBossSerializedProperty,
                new GUIContent(_propertyBossText));
            
            EditorGUILayout.LabelField(_labelWeaknessText, 
                GUILayout.Width(_labelWeaknessWidth));
            _bossWeaknessSerializedProperty.intValue = (int)(BossWeakness) EditorGUILayout.EnumPopup(
                (BossWeakness)_bossWeaknessSerializedProperty.intValue,
                GUILayout.MinWidth(_eNumWeaknessMinWidth));
            
            EditorGUILayout.EndHorizontal();
                
            // Editörde düşmek istediğiniz not, uyarı ya da hata gibi mesajları
            // HelpBox ile bırakabilirsiniz.
            EditorGUILayout.HelpBox(
                _helpBoxText,
                MessageType.Info);

            EditorGUILayout.EndToggleGroup();
#endregion

            // Inspector'da değiştirilen propertylerin uygulanması için
            // OnInspectorGUI'nin sonunda bu methodun çalıştırılması
            // gerekiyor.
            serializedObject.ApplyModifiedProperties();
        }
    }
}
