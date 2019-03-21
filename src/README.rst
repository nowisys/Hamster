Hamster
===========

This project defines the kernel for openHamster. The kernel is responsible to provide basic functionalities and integrate corresponding plugins in our openHAMSTER projects .
Plugins are responsible to implement a technical feature and/or define a business domain.

Usage
-----

The kernel allows to register additional plugins. Therefore a configuration has to be provided, which indicates directories containing assemblies (DLLs of plugins) and settings (XML files for plugin configuration).
The structure of spoken configurations can be found in the ProgramConfig_ class. openHAMSTER is searching for this configuration, named "hamster.xml", in a /etc folder from the current working directory upwards.
The path can be customized on startup by giving a console argument.

By default openHAMSTER is looking for assemblies and settings in following directories:

- Assemblies: lib/hamster/plugins
- Settings: [/]etc/hamster/plugins

These directories can be customized within the ProgramConfig_ as well.

.. _ProgramConfig: https://gitlab.hrz.tu-chemnitz.de/openhamster/Hamster/blob/master/src/Hamster/Configuration/ProgramConfig.cs