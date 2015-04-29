#Useful stuff for python controlof the Navigator experiment
# IB2015
#

clr.AddReference("System.Core")
import System
clr.ImportExtensions(System.Linq)
from System import *
from System.Collections.Generic import Dictionary,List

import os
import glob
from time import sleep

script_path = "C:\\EDMSuite\\NavMMScripts\\"
settings_path = "C:\\Data\\Settings\\NavigatorHardwareController\\"


#
# Useful functions
# Add things here that will be accessed from the console often. If 
# it takes more than a few lines put it in it's own file.
#
# ToDo:
# run a script multiple times, varying a parameter
#

def testRemote(dic):
	'''Tests remote stuff. Send it a dictionary and the console
	should print it out in nav hardwave control'''
	sendable = Dictionary[String, Object]()
	for k,v in dic.items():
		sendable.Add(k, v)
		print str(k) + ", " + str(v)
	returnString = hc.RemoteTest(sendable)

	
def SetChannel(channelName, value):
	'''Sets the channel with this name to value'''
	returnString = hc.RemoteSetChannel(channelName, value)
	if returnString != "":
		print returnString
		
def GetChannels():
	channels = hc.RemoteGetChannels()
	return [c for c in channels]
	
def GetScripts():
	return [os.path.basename(x) for x in glob.glob(script_path + "*.cs")]
	
def GetSavedParameters():
	return glob.glob(settings_path + "*.json")
	
def LoadParameters(file):
	returnString = hc.RemoteLoadParameters(file)
	if returnString != "":
		print returnString
		
def RunScript(scriptName, p={}, save=True, analyse = False):
	paramDict = Dictionary[String, Object]()
	for k,v in p.items():
		paramDict.Add(k, v)
	returnDic = dict(mm.RemoteRun(script_path + scriptName, paramDict, save))
	
	print returnDic["returnMessage"]
	
	if(analyse):
		AnalyseAbsImage(returnDic["Path"], returnDic["EID"])
	
	return returnDic
	
def AnalyseAbsImage(path, EID):
	anal.ComputeAbsImage(path + EID + ".zip", EID)
	
def close():
	try:
		hc.CloseIt()
	except:
		pass
		
	try:
		mm.CloseIt()
	except:
		pass
		
	try:	
		anal.CloseIt()
	except:
		pass
		
	exit()
	

f0 = "C:\\Data\\Nav\\data\\2015\\03\\27\\20150327_172746\\"
fz = "C:\\Data\\Nav\\data\\2015\\03\\27\\20150327_172746.zip"
f1 = "20150327_172746_0.png"
f2 = "20150327_172746_1.png"

t = lambda : anal.ComputeAbsImageFromFile(f0 + f1,f0 + f2)
v = lambda : anal.ComputeAbsImageFromZip(fz, f1, f2)

def findDelayTimes(openMax,closeMax,imDelay):
	for openTime in range(0,openMax,5):
		for closeTime in range(0,closeMax,5):
			RunScript("AbsImagingDelay.cs",{"imageDelay":imDelay,"openDelay":openTime,"closeDelay":closeTime},True)
			
def aomSweep(startFreq,endFreq):
	freqVal = startFreq
	while(1):
		if (freqVal<endFreq):
			SetChannel("aom1freq",freqVal)
			freqVal+=1.0
			sleep(0.2)
		else:
			freqVal = startFreq
			sleep(0.2)
def coilSweep(startCurrent,endCurrent,step=0.1,wait=0.2):
	coilVal = startCurrent
	while(1):
		if (coilVal>endCurrent):
			SetChannel("motCoil",coilVal)
			coilVal-=step
			sleep(wait)
		else:
			coilVal = startCurrent
			sleep(wait)
	