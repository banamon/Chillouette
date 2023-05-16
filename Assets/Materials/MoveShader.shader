Shader "Unlit/MoveShader"
{
    // Unity��ł���������v���p�e�B���
    // �}�e���A����Inspector�E�B���h�E��ɕ\������A�X�N���v�g�ォ����ݒ�ł���
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //_Color ("Main Color", Color) = (1,1,1,0.5) // Color �v���p�e�B�[ (�f�t�H���g�͔�)
    }

    // �T�u�V�F�[�_�[
    // �V�F�[�_�[�̎�ȏ����͂��̒��ɋL�q����
    // �T�u�V�F�[�_�[�͕����������Ƃ��\���A��{�͈��
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        // �p�X
        // 1�̃I�u�W�F�N�g��1�x�̕`��ōs�������������ɏ���
        // �������{������A���G�ȕ`�������Ƃ��͕����������Ƃ��\
        Pass
        {
            CGPROGRAM // �v���O�����������n�߂�Ƃ����錾

            // �֐��錾
            #pragma vertex vert
            #pragma fragment frag
            //#pragma fragment frag  // "frag" �֐����t���O�����g�V�F�[�_�[�Ǝg�p����錾


            //fixed4 _Color; // �}�e���A������̃J���[   a____


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
                float2 offset = float2(_Time.x*10, 0);//_Time����I�t�Z�b�g�����B�X�N���[���̕�����ς���ꍇ��y�ɑ��
                o.uv = TRANSFORM_TEX(v.uv + offset, _MainTex);//uv�ɃI�t�Z�b�g�����Z����
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }

            //            // �t���O�����g�V�F�[�_�[
            //fixed4 frag () : SV_Target
            //{
            //    return _Color;  // a____
            //}
 

            ENDCG // �v���O�����������I���Ƃ����錾
        }
    }
}
