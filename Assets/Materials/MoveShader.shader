Shader "Unlit/MoveShader"
{
    // Unity上でやり取りをするプロパティ情報
    // マテリアルのInspectorウィンドウ上に表示され、スクリプト上からも設定できる
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //_Color ("Main Color", Color) = (1,1,1,0.5) // Color プロパティー (デフォルトは白)
    }

    // サブシェーダー
    // シェーダーの主な処理はこの中に記述する
    // サブシェーダーは複数書くことも可能が、基本は一つ
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        // パス
        // 1つのオブジェクトの1度の描画で行う処理をここに書く
        // これも基本一つだが、複雑な描画をするときは複数書くことも可能
        Pass
        {
            CGPROGRAM // プログラムを書き始めるという宣言

            // 関数宣言
            #pragma vertex vert
            #pragma fragment frag
            //#pragma fragment frag  // "frag" 関数をフラグメントシェーダーと使用する宣言


            //fixed4 _Color; // マテリアルからのカラー   a____


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 offset = float2(_Time.x*10, 0);//_Timeからオフセットを作る。スクロールの方向を変える場合はyに代入
                o.uv = TRANSFORM_TEX(v.uv + offset, _MainTex);//uvにオフセットを加算する
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }

            //            // フラグメントシェーダー
            //fixed4 frag () : SV_Target
            //{
            //    return _Color;  // a____
            //}
 

            ENDCG // プログラムを書き終わるという宣言
        }
    }
}
