#pragma once

using namespace System;

namespace CameraHelper {
	public ref class CameraInfo {
	public:
		property int Id;
		property String^ Name;
	};

	public ref class CameraHelper {
	public:
		static System::Collections::Generic::List<CameraInfo^>^ GetCameras();
	};
}
