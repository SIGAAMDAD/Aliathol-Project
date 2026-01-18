#if UNITY_5_3_OR_NEWER || UNITY_2017_1_OR_NEWER || UNITY_2018_1_OR_NEWER
    #define USING_UNITY
    #define GAME_ENGINE
    #define HAS_SCENE_MANAGEMENT
    #define HAS_GAMEOBJECT_SYSTEM
#elif GODOT || GODOT4 || GODOT3
    #define USING_GODOT
    #define GAME_ENGINE
    #define HAS_NODE_SYSTEM
    #define HAS_SCENE_TREE
#elif UNREAL_ENGINE
    #define USING_UNREAL
    #define GAME_ENGINE
    #define HAS_ACTOR_COMPONENT
#elif MONOGAME || FNA
    #define USING_MONOGAME
    #define GAME_FRAMEWORK
    #define HAS_GAME_CLASS
#else
    #define USING_CUSTOM
    #define NO_ENGINE
#endif