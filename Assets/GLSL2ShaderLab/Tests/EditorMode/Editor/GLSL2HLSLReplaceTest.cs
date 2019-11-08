using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GLSL2HLSLTest
    {
        private List<string> _testLine = new List<string>()
        {
            "vec3 rect(vec2 bl, vec2 tr, vec2 st){",
            "float d = .02;",
            "vec3 r2 = vec3(blInv.x * trInv.x * blInv.y * trInv.y);",
            "	vec2 uv = (seed=32.+seed*fract(seed))+vec2(78.233, 10.873);",
            "            rotmat = mat3(cs, -sn, 0.0,",
            "    vec2 mo = iMouse.xy/iResolution.xy;"
        };
        
        [Test]
        public void GLSL2HLSLTestSimplePasses()
        {
            var converter = new Converter();
            converter.Start();
        }

        [Test]
        public void ReplaceGlsl2HlslLineTest1()
        {
            var converter = new Converter();
            var line = converter.ReplaceGlsl2Hlsl(_testLine[0]);
            Assert.AreEqual("float3 rect(float2 bl, float2 tr, float2 st){", line);
        }

        [Test]
        public void ReplaceGlsl2HlslLineTest2()
        {
            var converter = new Converter();
            var line = converter.ReplaceGlsl2Hlsl(_testLine[1]);
            Assert.AreEqual("float d = .02;", line);
        }
        
        [Test]
        public void ReplaceGlsl2HlslLineTest3()
        {
            var converter = new Converter();
            var line = converter.ReplaceGlsl2Hlsl(_testLine[2]);
            Assert.AreEqual("float3 r2 = float3(blInv.x * trInv.x * blInv.y * trInv.y);", line);
        }
        
        [Test]
        public void ReplaceGlsl2HlslLineTest4()
        {
            var converter = new Converter();
            var line = converter.ReplaceGlsl2Hlsl(_testLine[3]);
            Assert.AreEqual("	float2 uv = (seed=32.+seed*frac(seed))+float2(78.233, 10.873);", line);
        }
        [Test]
        public void ReplaceGlsl2HlslLineTest5()
        {
            var converter = new Converter();
            var line = converter.ReplaceGlsl2Hlsl(_testLine[4]);
            Assert.AreEqual("            rotmat = float3x3(cs, -sn, 0.0,", line);
        }
        
        [Test]
        public void ReplaceGlsl2HlslLineTest6()
        {
            var converter = new Converter();
            var line = converter.ReplaceGlsl2Hlsl(_testLine[5]);
            Assert.AreEqual("    float2 mo = float2(1.0, 1.0)/_ScreenParams.xy;", line);
        }
    }
}
