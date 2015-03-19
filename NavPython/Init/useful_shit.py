#Useful stuff for python controlof the Navigator experiment
# IB2015
#

clr.AddReference("System.Core")
import System
clr.ImportExtensions(System.Linq)
from System import *
from System.Collections.Generic import Dictionary,List
import glob


script_path = "NavMMScripts\\"
settings_path = "C:\\Data\\Settings\\NavigatorHardwareController\\"


#
# Useful functions
# Add things here that will be accessed from the console often. If 
# it takes more than a few lines put it in it's own file.
#
# ToDo:
# run scripts (options?)
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
	return glob.glob(script_path + "*.cs")
	
def GetSavedParameters():
	return glob.glob(settings_path + "*.json")
	
def LoadParameters(file):
	returnString = hc.RemoteLoadParameters(file)
	if returnString != "":
		print returnString
		
def RunScript(scriptName, parameters, save=False):
	paramDict = Dictionary[String, Object]()
	for k,v in parameters.items():
		paramDict.Add(k, v)
	returnString = mm.RemoteRun(scriptName, paramDict, save)
	print returnString
	