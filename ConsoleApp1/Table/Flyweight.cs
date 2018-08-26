using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace NTable
{
    public abstract class Flyweight
    {
        // 해당 테이블이 발동될 명령어를 세팅합니다. 중복될 수 없습니다.
        public abstract string GetFlyweightType();

        // 해당 테이블의 경로를 세팅합니다.
        public abstract string GetFilePath();

        // 테이블 빌드를 실행합니다.
        public abstract bool Execute(string fileName, bool makeClear = false);

        // 테이블의 크기를 불러옵니다.
        public abstract int GetSize();

        // 테이블의 현재 데이터 갯수를 가져옵니다.
        public abstract int GetCount();

        // 테이블에서 응답을 가져옵니다.
        public abstract string GetItem();

        // 기본값 : 테이블을 빌드합니다.
        public void TableBuild()
        {
            this.Execute(GetFilePath());
        }
    }
}