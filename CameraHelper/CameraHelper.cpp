#include "pch.h"

#include <Windows.h>
#include <strmif.h>
#include <dshow.h>

#include "CameraHelper.h"


using namespace System;
using namespace System::Collections::Generic;
using namespace CameraHelper;

List<CameraInfo^>^ ::CameraHelper::CameraHelper::GetCameras() {
    auto list = gcnew List<CameraInfo^>;

    HRESULT hr;
    ICreateDevEnum* pSysDevEnum;
    hr = CoCreateInstance(CLSID_SystemDeviceEnum, NULL, CLSCTX_INPROC_SERVER,
        IID_ICreateDevEnum, (void**)&pSysDevEnum);
    if (FAILED(hr)) {
        return list;
    }

    IEnumMoniker *pEnumCat;
    hr = pSysDevEnum->CreateClassEnumerator(CLSID_VideoInputDeviceCategory, &pEnumCat, 0);
    if (hr != S_OK) {
        pSysDevEnum->Release();
        return list;
    }

    IMoniker *pMoniker;
    ULONG cFetched;
    auto id = 0;
    while(pEnumCat->Next(1, &pMoniker, &cFetched) == S_OK) {
        IPropertyBag* pPropBag;
        hr = pMoniker->BindToStorage(0, 0, IID_IPropertyBag, (void**)&pPropBag);
        if (SUCCEEDED(hr)) {
            VARIANT varName;
            VariantInit(&varName);
            hr = pPropBag->Read(L"FriendlyName", &varName, 0);
            if (SUCCEEDED(hr)) {
                auto info = gcnew CameraInfo;
                info->Id = id++;
                info->Name = gcnew String(varName.bstrVal);
                list->Add(info);
            }
            VariantClear(&varName);
            pPropBag->Release();
        }
        pMoniker->Release();
    }
    pEnumCat->Release();

    return list;
}